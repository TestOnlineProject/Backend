using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IUserService
    {
        Task CreateUser(UserCreateDto userToCreate);
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(string id);
        Task DeleteUser(string id);
        Task UpdateUser(UserDto userToUpdate);
        Task RequestToTakeTheExam(string userId, int examId);
        Task ApproveExam(string userId, int examId, string adminId);
        Task<User?> GetByEmail(string email);


    }
}