using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IUserService
    {
        Task CreateUser(UserCreateDto userToCreate);
        Task DeleteUser(int id);
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(int id);
        Task UpdateUser(UserDto userToUpdate);
        Task RequestToTakeTheExam(int userId, int examId);
        Task ApproveExam(int userId, int examId, int adminId);

    }
}