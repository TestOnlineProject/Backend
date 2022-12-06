using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestOnline.Models.Dtos.Question;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IQuestionService
    {
        Task<Question> GetQuestion(int id);
        Task<List<Question>> GetAllQuestions();
        Task CreateQuestion(QuestionCreateDto questionToCreate);
        Task CreateQuestions(List<QuestionCreateDto> questionsToCreate);
        Task CreateQuestionsFromFile(IFormFile file);
        Task DeleteQuestion(int id);
        Task<string> UploadImageFromUrl(string url, int questionId);
        Task<string> UploadImage(IFormFile? file, int questionId);
    }
}
