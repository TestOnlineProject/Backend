using Amazon.S3.Model;
using Amazon.S3;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TestOnline.Data.UnitOfWork;
using TestOnline.Models.Dtos.Question;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Amazon.S3.Util.S3EventNotification;
using TestOnline.Data;

namespace TestOnline.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dbContext;



        public async Task<string> GetRole(ClaimsIdentity userClaims)
        {
            var userId = userClaims.Claims.First(x => x.Type.Equals("Id")).Value;

            var user = await _unitOfWork.Repository<User>().GetByCondition(x => x.UserId == userId).FirstOrDefaultAsync();
            return user.Role;
        }

        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, DataContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<Question> GetQuestion(int id)
        {
            Expression<Func<Question, bool>> expression = x => x.QuestionId == id;
            var question = await _unitOfWork.Repository<Question>().GetById(expression).FirstOrDefaultAsync();

            return question;
        }

        public async Task<List<Question>> GetAllQuestions()
        {
            var questions = _unitOfWork.Repository<Question>().GetAll();
            return questions.ToList();
        }

        
        public async Task CreateQuestion(QuestionCreateDto questionToCreate)
        {
            var question = _mapper.Map<Question>(questionToCreate);

            _unitOfWork.Repository<Question>().Create(question);

            _unitOfWork.Complete();
        }

       

        public async Task CreateQuestions(List<QuestionCreateDto> questionsToCreate, ClaimsIdentity userClaims)
        {
            var role = await GetRole(userClaims);
            var questions = _mapper.Map<List<QuestionCreateDto>, List<Question>>(questionsToCreate);
            if (role != "Admin")
            {
                throw new Exception("Permission Denied: Only Admin can create questions.");
            }
            _unitOfWork.Repository<Question>().CreateRange(questions);
            _unitOfWork.Complete();
        }
        public async Task DeleteQuestion(int id)
        {
            var question = await GetQuestion(id);
            if (question == null)
            {
                throw new NullReferenceException("The question you're trying to delete doesn't exist.");
            }

            _unitOfWork.Repository<Question>().Delete(question);

            _unitOfWork.Complete();

        }


        public async Task UploadImage(IFormFile file, int id)
        {
            var uploadPicture = await UploadToBlob(file);
            var imageUrl = $"{_configuration.GetValue<string>("BlobConfig:CDNLife")}{file.FileName}";

            var question = await GetQuestion(id);
            question.ImageSrc = imageUrl;

            _unitOfWork.Repository<Question>().Update(question);
            _unitOfWork.Complete();
        }

        public async Task<PutObjectResponse> UploadToBlob(IFormFile file)
        {
            string serviceURL = _configuration.GetValue<string>("BlobConfig:serviceURL");
            string AWS_accessKey = _configuration.GetValue<string>("BlobConfig:accessKey");
            string AWS_secretKey = _configuration.GetValue<string>("BlobConfig:secretKey");
            var bucketName = _configuration.GetValue<string>("BlobConfig:bucketName");
            var keyName = _configuration.GetValue<string>("BlobConfig:defaultFolder");

            var config = new AmazonS3Config() { ServiceURL = serviceURL };
            var s3Client = new AmazonS3Client(AWS_accessKey, AWS_secretKey, config);
            keyName = String.Concat(keyName, file.FileName);

            var fs = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = fs,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };
            return await s3Client.PutObjectAsync(request);
        }
    }
}
