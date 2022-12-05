using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestOnline.Models.Dtos.Exam;
using TestOnline.Models.Entities;
using TestOnline.Services;
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

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("Exam/GetQuestions")]
        public async Task<List<Question>?> GetExamQuestions(int id)
        {
            var questions = await _examService.GetExamQuestions(id);
            return questions;
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("GetExams")]
        public async Task<IActionResult> GetExams()
        {
            var exams = await _examService.GetAllExams();
            return Ok(exams);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost("CreateExam")]
        public async Task<IActionResult> CreateExam(int nrOfQuestions, string name)
        {
            await _examService.CreateExam(nrOfQuestions, name);
            return Ok("Exam created successfully!");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPut("UpdateExam")]
        public async Task<IActionResult> UpdateExam(ExamDto examToUpdate)
        {
            if (examToUpdate is null)
            {
                return BadRequest("You havent provided the examToUpdate!");
            }
            await _examService.UpdateExam(examToUpdate);
            return Ok("Exam updated successfully!");

        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, User")]
        [HttpPost("RequestToTakeTheExam")]
        public async Task<IActionResult> RequestToTakeTheExam(string userId, int examId)
        {
            try
            {
                await _examService.RequestToTakeTheExam(userId, examId);
                return Ok("Request for taking the exam is created successfully!");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during the request to take the exam");
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

        }


        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPut("ApproveExam")]
        public async Task<IActionResult> ApproveExam(string userId, int examId)
        {
            try
            {
                await _examService.ApproveExam(userId, examId);
                return Ok($"The exam request for user with Id:{userId} was approved successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during the approval of exam.");
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, User")]
        [HttpGet("StartExam")]
        public async Task<List<Question>> StartExam(int examId)
        {
            var claims = User.Identity as ClaimsIdentity;
            var userId = claims.Claims.First(x => x.Type.Equals("Id")).Value;
            try
            {
                var questions = await _examService.StartExam(userId, examId);
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in the process of starting the exam!");
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, User")]
        [HttpPost("SubmitExam")]
        public async Task<double> SubmitExam(int examId, List<int> answers)
        {
            var claims = User.Identity as ClaimsIdentity;
            var userId = claims.Claims.First(x => x.Type.Equals("Id")).Value;
            try
            {
                var result = await _examService.SubmitExam(examId, userId, answers);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in the process of submiting the exam!");
                Console.WriteLine(ex.Message);
                return 0.0;
            }
        }
    }
}
