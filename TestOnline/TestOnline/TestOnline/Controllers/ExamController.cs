using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestOnline.Models.Dtos.Exam;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;

namespace TestOnline.Controllers
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExamController : Controller
    {
        private readonly IExamService _examService;
        private readonly IQuestionService _questionService;
        private readonly ILogger<ExamController> _logger;

        public ExamController(IExamService examService, ILogger<ExamController> logger, IQuestionService questionService)
        {
            _examService = examService;
            _logger = logger;
            _questionService = questionService;
        }


        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            //await _emailSender.SendEmailAsync("albion.b@gjirafa.com", "Hello From Life", "Content");


            //try
            //{
            int num = 4;
            int num2 = 0;

            int num3 = num / num2;
            //}
            //catch(Exception ex)
            //{
            //    _logger.LogError(ex, "Error i LIFE");
            //    _logger.LogInformation(ex, "Error i LIFE");
            //    _logger.LogDebug(ex, "Error i LIFE");
            //}

            return Ok();
            //return Ok($"{exam} {exam1}");
        }

        [HttpGet("Exam/GetQuestions")]
        public async Task<List<Question>?> GetExamQuestions(int id)
        {
            var questions = await _examService.GetExamQuestions(id);
            return questions;
        }

        [HttpGet("GetExam")]
        public async Task<IActionResult> Get(int id)
        {
            var exam = await _examService.GetExam(id);

            if (exam == null)
            {
                return NotFound();
            }

            return Ok(exam);
        }

        [HttpGet("GetExams")]
        public async Task<IActionResult> GetExams()
        {
            var exams = await _examService.GetAllExams();

            return Ok(exams);
        }


        [HttpPost("CreateExam")]
        public async Task<IActionResult> CreateExam(int nrOfQuestions, string name)
        {
            await _examService.CreateExam(nrOfQuestions, name);

            return Ok("Exam created successfully!");
        }


        [HttpPut("UpdateExam")]
        public async Task<IActionResult> Update(ExamDto ExamToUpdate)
        {
            try
            {
                await _examService.UpdateExam(ExamToUpdate);
                return Ok("Exam updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during the update process.");
                return BadRequest("The exam specified doesn't exist!");
            }
        }

        [HttpDelete("DeleteExam")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _examService.DeleteExam(id);
                return Ok("Exam deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during the deletion process.");
                return BadRequest("The exam specified doesnt't exist!");
            }
        }

        [HttpGet("StartExam")]  
        public async Task<List<Question>> StartExam(string userId, int examId)
        {
            var questions = await _examService.StartExam(userId, examId);
            return questions;
        }

        [HttpPost("SubmitExam")]
        public async Task<double> SubmitExam(int examId, List<int> answers)
        {
            var result = await _examService.SubmitExam(examId, answers);
            return result;
        }


    }
}
