using TestOnline.Models.Dtos.Exam;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IExamService
    {
        Task DeleteExam(int id);
        Task<List<Exam>> GetAllExams();
        //Task<PagedInfo<Exam>> ExamsListView(string search, int page, int pageSize);
        Task<Exam> GetExam(int id);
        Task CreateExam(int nrOfQuestions, string name);
        Task UpdateExam(ExamDto examToUpdate);
        Task<List<Question>> GetExamQuestions(int id);
        Task<List<Question>> StartExam(int userId, int examId);

    }
}
