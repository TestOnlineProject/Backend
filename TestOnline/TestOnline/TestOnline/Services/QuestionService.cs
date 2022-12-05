using Amazon.S3.Model;
using Amazon.S3;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TestOnline.Data.UnitOfWork;
using TestOnline.Models.Dtos.Question;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Amazon.S3.Util.S3EventNotification;
using TestOnline.Data;
using static System.IO.Stream;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using SendGrid;

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
        }



        public async Task CreateQuestions(List<QuestionCreateDto> questionsToCreate)
        {
            var questions = _mapper.Map<List<QuestionCreateDto>, List<Question>>(questionsToCreate);
            _unitOfWork.Repository<Question>().CreateRange(questions);
            _unitOfWork.Complete();
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

        public System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;
            _logger.LogInformation($"Downloading image from url: {imageUrl}");
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
            webRequest.AllowWriteStreamBuffering = true;
            webRequest.Timeout = 15000;

            System.Net.WebResponse webResponse = webRequest.GetResponse();

            System.IO.Stream stream = webResponse.GetResponseStream();
            var fsr = new FileStreamResult(stream, "image/png");
            GetFormFile(fsr);
            image = System.Drawing.Image.FromStream(stream);
           
            webResponse.Close();
            return image;
        }
        public async Task GetFormFile(FileStreamResult fsr)
        {
            using (var fs = fsr.FileStream)
            {
                var file = new FormFile(fs, 0, fs.Length, "name", fsr.FileDownloadName);
                await UploadImage(file, 1);
            }
        }
        public async Task UploadImageFromUrl(string url, int questionId)
        {
            System.Drawing.Image image = DownloadImageFromUrl(url.Trim());

            
            //await UploadImage((IFormFile)image, questionId);
            _logger.LogInformation($"Uploading image from url: {url}");

            string rootPath = @"C:\Users\DELL\Desktop\Images";
            string fileName = System.IO.Path.Combine(rootPath, "testblah.gif");
            image.Save(fileName);
            #region SelectPdf
            //string url = TxtUrl.Text;

            //string image_format = DdlImageFormat.SelectedValue;
            //ImageFormat imageFormat = ImageFormat.Png;
            //if (image_format == "jpg")
            //{
            //    imageFormat = ImageFormat.Jpeg;
            //}
            //else if (image_format == "bmp")
            //{
            //    imageFormat = ImageFormat.Bmp;
            //}

            //int webPageWidth = 1024;
            //try
            //{
            //    webPageWidth = Convert.ToInt32(TxtWidth.Text);
            //}
            //catch { }

            //int webPageHeight = 0;
            //try
            //{
            //    webPageHeight = Convert.ToInt32(TxtHeight.Text);
            //}
            //catch { }

            //// instantiate a html to image converter object
            //HtmlToImage imgConverter = new HtmlToImage();

            //// set converter options
            //imgConverter.WebPageWidth = webPageWidth;
            //imgConverter.WebPageHeight = webPageHeight;

            //// create a new image converting an url
            //System.Drawing.Image image = imgConverter.ConvertUrl(url);

            //// send image to browser
            //Response.Clear();
            //Response.ClearHeaders();
            //Response.AddHeader("Content-Type", "image/" +
            //    imageFormat.ToString().ToLower());
            //Response.AppendHeader("content-disposition",
            //    "attachment;filename=\"image." + image_format + "\"");
            //image.Save(Response.OutputStream, imageFormat);
            //Response.End();
            #endregion
        }
        public async Task UploadImage2(string url, int questionId)
        {
            await UploadImage(file: null, id: questionId, url);
        }

        public async Task<string> UploadImage(IFormFile? file, int id, string url = null)
        {
            String imageUrl = "";
            if (url is not null)
            {
                var uploadPicture = await UploadToBlob(file: null, url);
                imageUrl = $"{_configuration.GetValue<string>("BlobConfig:CDNLife")}{url}";
            }
            else
            {
                var uploadPicture = await UploadToBlob(file);
                imageUrl = $"{_configuration.GetValue<string>("BlobConfig:CDNLife")}{file.FileName}";
            }

            var question = await GetQuestion(id);
            if (question is null)
            {
                throw new NullReferenceException("There is no question with the given id!");
            }
            question.ImageSrc = imageUrl;

            _unitOfWork.Repository<Question>().Update(question);
            _unitOfWork.Complete();
            return imageUrl;
        }



        public async Task<PutObjectResponse> UploadToBlob(IFormFile? file, string url = null)
        {
            string serviceURL = _configuration.GetValue<string>("BlobConfig:serviceURL");
            string AWS_accessKey = _configuration.GetValue<string>("BlobConfig:accessKey");
            string AWS_secretKey = _configuration.GetValue<string>("BlobConfig:secretKey");
            var bucketName = _configuration.GetValue<string>("BlobConfig:bucketName");
            var keyName = _configuration.GetValue<string>("BlobConfig:defaultFolder");
            var config = new AmazonS3Config() { ServiceURL = serviceURL };
            var s3Client = new AmazonS3Client(AWS_accessKey, AWS_secretKey, config);
            Stream? fs = null;
            String contentType = "";

            if (url is not null)
            {
                contentType = "image/png";
                keyName = url;

                using (var httpClient = new HttpClient())
                {
                    //Issue the GET request to a URL and read the response into a 
                    //stream that can be used to load the image
                    var imageContent = await httpClient.GetByteArrayAsync(url);

                    using (var imageBuffer = new MemoryStream(imageContent))
                    {
                        var image = System.Drawing.Image.FromStream(imageBuffer);
                        fs = imageBuffer;

                    }
                }
            }
            else
            {
                fs = file.OpenReadStream();
                keyName = String.Concat(keyName, file.FileName);
                contentType = file.ContentType;
            }
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = fs,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };
            _logger.LogInformation("File is uploaded to blob successfully!");
            return await s3Client.PutObjectAsync(request);
        }
    }
}
