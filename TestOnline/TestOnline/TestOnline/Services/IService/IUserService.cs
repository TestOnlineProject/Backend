using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;

namespace TestOnline.Services.IService
{
    public interface IUserService
    {
        Task CreateUser(UserCreateDto userToCreate);
        Task DeleteUser(int id);
        Task<List<User>> GetAllUsers();
        //Task<PagedInfo<User>> UsersListView(string search, int page, int pageSize);
        Task<User> GetUser(int id);
        Task UpdateUser(UserDto userToUpdate);
    }
}