using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TestOnline.Models.Dtos.Question;
using TestOnline.Services.IService;

namespace TestOnline.Controllers
{
    [Route("api")]
    [ApiController]
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

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("GetQuestions")]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _questionService.GetAllQuestions();

            return Ok(questions);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost("CreateQuestion")]
        public async Task<IActionResult> Create(QuestionCreateDto questionToCreate)
        {
            await _questionService.CreateQuestion(questionToCreate);
            return Ok("Question created successfully!");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost("CreateQuestions")]
        public async Task<IActionResult> CreateQuestions(List<QuestionCreateDto> questionsToCreate)
        {
            await _questionService.CreateQuestions(questionsToCreate);
            return Ok("Questions are created successfully");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost("CreateQuestionsFromFile")]
        public async Task<IActionResult> CreateQuestionsFromFile(IFormFile file)
        {
            await _questionService.CreateQuestionsFromFile(file);
            return Ok("Questions from file are created successfully!");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, User")]
        public async Task<IActionResult> UploadImageFromUrl(int questionId, string url)
        {
            var imageUrl = await _questionService.UploadImageFromUrl(url, questionId);
            return Ok($"Image is downloaded and uploaded to blob successfully at the url: {imageUrl}");
        }


        [HttpPost("UploadImage")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, User")]
        public async Task<IActionResult> UploadImage(IFormFile file, int id)
        {
            var url = await _questionService.UploadImage(file, id);
            return Ok($"Picture was uploaded sucessfully at the url: {url}");
        }
    }
}
