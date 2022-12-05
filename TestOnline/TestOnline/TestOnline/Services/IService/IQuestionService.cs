using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestOnline.Models.Dtos.Question;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IQuestionService
    {
        Task CreateQuestion(QuestionCreateDto questionToCreate);
        Task DeleteQuestion(int id);
        Task<List<Question>> GetAllQuestions();
        Task<Question> GetQuestion(int id);
        Task CreateQuestions(List<QuestionCreateDto> questionsToCreate);
        Task CreateQuestionsFromFile(IFormFile file);
        Task<string> UploadImage(IFormFile? file, int questionId);
        Task<string> UploadImageFromUrl(string url, int questionId);
        Task<PutObjectResponse> UploadToBlob(IFormFile? file, string name, string extension);





    }
}
