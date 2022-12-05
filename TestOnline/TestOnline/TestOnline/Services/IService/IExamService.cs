using TestOnline.Models.Dtos.Exam;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IExamService
    {
        Task DeleteExam(int id);
        Task<List<Exam>> GetAllExams();
        Task<Exam> GetExam(int id);
        Task CreateExam(int nrOfQuestions, string name);
        Task UpdateExam(ExamDto examToUpdate);
        Task<List<Question>> GetExamQuestions(int id);
        Task<List<Question>> StartExam(string userId, int examId);
        Task<double> SubmitExam(int examId, string userId, List<int> answers);
        Task RequestToTakeTheExam(string userId, int examId);
        Task ApproveExam(string userId, int examId);


    }
}
