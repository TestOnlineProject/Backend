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

        public QuestionController(ILogger<QuestionController> logger, IQuestionService questionService)
        {
            _logger = logger;
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
            try
            {
                await _questionService.CreateQuestionsFromFile(file);
                return Ok("Questions from file are created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in creating questions from file.");
                return BadRequest(ex.Message);
            }
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
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UploadImageFromUrl")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, User")]
        public async Task<IActionResult> UploadImageFromUrl(int questionId, string url)
        {
            try
            {
                var imageUrl = await _questionService.UploadImageFromUrl(url, questionId);
                return Ok($"Image is downloaded and uploaded to blob successfully at the url: {imageUrl}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in uploading the image from url.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UploadImage")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, User")]
        public async Task<IActionResult> UploadImage(IFormFile file, int questionId)
        {
            try
            {
                var url = await _questionService.UploadImage(file, questionId);
                return Ok($"Picture was uploaded sucessfully at the url: {url}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in uploading the image.");
                return BadRequest(ex.Message);
            }

        }
    }
}
