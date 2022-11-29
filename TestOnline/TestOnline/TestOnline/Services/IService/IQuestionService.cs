using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
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
        Task UploadImage(IFormFile file, int id);
        Task<PutObjectResponse> UploadToBlob(IFormFile file);
    }
}
