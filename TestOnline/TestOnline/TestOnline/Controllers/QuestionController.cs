using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TestOnline.Data;
using TestOnline.Models.Dtos.Question;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;
using TestOnline.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TestOnline.Controllers
{
    [Route("api")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;
        private readonly IEmailSender _emailSender;

        public QuestionController(ILogger<QuestionController> logger, IEmailSender emailSender, IQuestionService questionService)
        {
            _logger = logger;
            _emailSender = emailSender;
            _questionService = questionService;
        }
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        [HttpGet("GetQuestion")]
        public async Task<IActionResult> Get(int id)
        {
            var question = await _questionService.GetQuestion(id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }


        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("GetQuestions")]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _questionService.GetAllQuestions();
            
            return Ok(questions);
        }
        [HttpPost("CreateQuestion")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> Create(QuestionCreateDto QuestionToCreate)
        {
            await _questionService.CreateQuestion(QuestionToCreate);
            return Ok("Question created successfully!");
        }

        [HttpPost("CreateQuestions")]
        public async Task<IActionResult> CreateQuestions(List<QuestionCreateDto> questionsToCreate)
        {
            await _questionService.CreateQuestions(questionsToCreate);
            return Ok("Questions are created successfully");

        }

        [HttpPost("CreateQuestionsFromFile")]
        public async Task<IActionResult> CreateQuestionsFromFile(IFormFile file)
        {
            await _questionService.CreateQuestionsFromFile(file);
            return Ok("Questions from file are created successfully!");
        }

        [HttpDelete("DeleteQuestion")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _questionService.DeleteQuestion(id);
                return Ok("Question deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during the deletion process.");
                return BadRequest("The question specified doesnt't exist!");
            }
        }

        [HttpPost("UploadImageFromUrl")]
        public async Task<IActionResult> UploadImageFromUrl(string url, int questionId)
        {
            await _questionService.UploadImageFromUrl(url, questionId);
            return Ok("Image is downloaded and uploaded to blob successfully!");
        }
        [HttpPost("UploadImage2")]
        public async Task<IActionResult> UploadImage2(int questionId, [FromBody] string url)
        {
            await _questionService.UploadImage2(url, questionId);
            return Ok("Image is downloaded and uploaded to blob successfully!");
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file, int id)
        {
            var url =  await _questionService.UploadImage(file, id);
            return Ok($"Picture was uploaded sucessfully at the url: {url}!");
        }
    }
}
