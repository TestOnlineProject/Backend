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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet("GetQuestions")]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _questionService.GetAllQuestions();

            return Ok(questions);
        }

        [HttpPost("CreateQuestion")]
        public async Task<IActionResult> Create(QuestionCreateDto QuestionToCreate)
        {
            await _questionService.CreateQuestion(QuestionToCreate);
            return Ok("Question created successfully!");
        }

        [HttpPost("CreateQuestions")]
        public async Task<IActionResult> CreateQuestions(List<QuestionCreateDto> questionsToCreate)
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims == null)
            {
                throw new Exception("User request is not valid.");
            }
            await _questionService.CreateQuestions(questionsToCreate, userClaims);
            return Ok("Questions were added successfully!");

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

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file, int id)
        {
            await _questionService.UploadImage(file, id);
            return Ok("Picture was uploaded sucessfully!");
        }
    }
}
