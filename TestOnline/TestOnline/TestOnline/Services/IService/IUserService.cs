using System.Drawing;
using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IUserService
    {
        Task<User> GetUser(string id);
        Task<List<User>> GetAllUsers();
        Task UpdateUser(UserDto userToUpdate);
        Task DeleteUser(string id);
        Task<User?> GetByEmail(string email);
        Task SendEmailOnRegistration(string email, string firstName);
    }
}