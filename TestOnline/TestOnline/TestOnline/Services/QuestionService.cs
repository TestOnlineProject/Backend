using Amazon.S3.Model;
using Amazon.S3;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TestOnline.Data.UnitOfWork;
using TestOnline.Models.Dtos.Question;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace TestOnline.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<QuestionService> _logger;


        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<QuestionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
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
            _logger.LogInformation("Created question successfully!");

        }


        public async Task CreateQuestions(List<QuestionCreateDto> questionsToCreate)
        {
            var questions = _mapper.Map<List<QuestionCreateDto>, List<Question>>(questionsToCreate);
            _unitOfWork.Repository<Question>().CreateRange(questions);
            _unitOfWork.Complete();
            _logger.LogInformation("Created questions successfully!");

        }


        public async Task CreateQuestionsFromFile(IFormFile file)
        {
            #region Creating questions from a given path
            // var lines = File.ReadAllText(path);
            // var questions = JsonConvert.DeserializeObject<List<QuestionCreateDto>>(lines);
            #endregion 

            if (file == null || file.Length == 0)
            {
                throw new Exception("The file given is null or empty!");
            }
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var lines = reader.ReadToEnd();
                var questions = JsonConvert.DeserializeObject<List<QuestionCreateDto>>(lines);
                await CreateQuestions(questions);
            }
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


        public async Task<string> UploadImage(IFormFile? file, int questionId)
        {
            var uploadPicture = await UploadToBlob(file, file.FileName, Path.GetExtension(file.FileName));
            var imageUrl = $"{_configuration.GetValue<string>("BlobConfig:CDNLife")}{file.FileName}";

            var question = await GetQuestion(questionId);
            if (question is null)
            {
                throw new NullReferenceException("There is no question with the given id!");
            }
            question.ImageSrc = imageUrl;

            _unitOfWork.Repository<Question>().Update(question);
            _unitOfWork.Complete();
            return imageUrl;
        }


        public async Task<string> UploadImageFromUrl(string url, int questionId)
        {
            if (url.IsNullOrEmpty())
            {
                throw new Exception("The url can't be null or empty!");
            }

            var httpClient = new HttpClient();
            HttpResponseMessage res = await httpClient.GetAsync(url.Replace("%2F", "/"));
            byte[] content = await res.Content.ReadAsByteArrayAsync();
            var extension = Path.GetExtension(url);
            var imageUri = Guid.NewGuid() + extension;
            var stream = new MemoryStream(content);
            IFormFile file = new FormFile(stream, 0, content.Length, null, imageUri);
            await UploadToBlob(file, imageUri, extension);
            var imageInCdnUrl = $"{_configuration.GetValue<string>("BlobConfig:CDNLife")}{imageUri}";

            var question = await GetQuestion(questionId);
            if (question is null)
            {
                throw new NullReferenceException("There is no question with the given id!");
            }

            question.ImageSrc = imageInCdnUrl;
            return imageInCdnUrl;
        }


        public async Task<PutObjectResponse> UploadToBlob(IFormFile? file, string name, string extension)
        {
            string serviceURL = _configuration.GetValue<string>("BlobConfig:serviceURL");
            string AWS_accessKey = _configuration.GetValue<string>("BlobConfig:accessKey");
            string AWS_secretKey = _configuration.GetValue<string>("BlobConfig:secretKey");
            var bucketName = _configuration.GetValue<string>("BlobConfig:bucketName");
            var keyName = _configuration.GetValue<string>("BlobConfig:defaultFolder");

            var config = new AmazonS3Config() { ServiceURL = serviceURL };
            var s3Client = new AmazonS3Client(AWS_accessKey, AWS_secretKey, config);
            keyName = String.Concat(keyName, name);

            var fs = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = fs,
                ContentType = $"image/{extension}",
                CannedACL = S3CannedACL.PublicRead
            };
            _logger.LogInformation("File is uploaded to blob successfully!");
            return await s3Client.PutObjectAsync(request);
        }
    }
}
