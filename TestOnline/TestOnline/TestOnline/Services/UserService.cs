using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System.Linq.Expressions;
using TestOnline.Data.UnitOfWork;
using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;

namespace TestOnline.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = _unitOfWork.Repository<User>().GetAll();
            return users.ToList();
        }

        public async Task<User> GetUser(int id)
        {
            Expression<Func<User, bool>> expression = x => x.UserId == id;
            var user = await _unitOfWork.Repository<User>().GetById(expression).FirstOrDefaultAsync();

            return user;
        }

        public async Task UpdateUser(UserDto userToUpdate)
        {
            User? user = await GetUser(userToUpdate.UserId);

            user.FirstName = userToUpdate.FirstName;
            user.LastName = userToUpdate.LastName;
            user.Location = userToUpdate.Location;
            user.Role = userToUpdate.Role;


            _unitOfWork.Repository<User>().Update(user);

            _unitOfWork.Complete();
        }

        public async Task DeleteUser(int id)
        {
            var user = await GetUser(id);

            _unitOfWork.Repository<User>().Delete(user);

            _unitOfWork.Complete();
        }

        public async Task CreateUser(UserCreateDto userToCreate)
        {
            var user = _mapper.Map<User>(userToCreate);

            _unitOfWork.Repository<User>().Create(user);

            _unitOfWork.Complete();
        }
    }
}
