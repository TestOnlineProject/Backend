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
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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
            User user = await GetUser(userToUpdate.UserId);
            if (user == null)
            {
                throw new NullReferenceException("The user specified doesn't exist.");
            }

            user.FirstName = userToUpdate.FirstName;
            user.LastName = userToUpdate.LastName;
            user.Location = userToUpdate.Location;
            user.Role = userToUpdate.Role;

            _unitOfWork.Repository<User>().Update(user);

            _unitOfWork.Complete();
        }   

        public async Task CreateUser(UserCreateDto userToCreate)
        {
            var user = _mapper.Map<User>(userToCreate);

           
            _unitOfWork.Repository<User>().Create(user);
            _logger.LogInformation("Created user successfully!");
            _unitOfWork.Complete();
        }

        public async Task DeleteUser(int id)
        {
            var user = await GetUser(id);
            if (user == null)
            {
                throw new NullReferenceException("The user you're trying to delete doesn't exist.");
            }

            _unitOfWork.Repository<User>().Delete(user);
            _logger.LogInformation("Deleted user successfully!");
            _unitOfWork.Complete();
        }
       

        public async Task RequestToTakeTheExam(int userId, int examId)
        {
            var user = await GetUser(examId);
            if (user == null)
            {
                throw new NullReferenceException("The user who requested to take the exam is not registered!");
            }
            var exam = await _unitOfWork.Repository<Exam>().GetById(x => x.ExamId == examId).FirstOrDefaultAsync();

            if (exam == null)
            {
                throw new NullReferenceException("The exam specified doesn't exist.");
            }

            ExamUser examUser = new ExamUser
            {
                User = user,
                UserId = userId,

                Exam = exam,
                ExamId = examId
            };
            _unitOfWork.Repository<ExamUser>().Create(examUser);
            _unitOfWork.Complete();
        }

        public async Task ApproveExam(int userId, int examId, int adminId) // userId is the id of the user who requested to take the exam, adminId is the id of the user who should be admin
        {
            var applyingUser = await GetUser(userId);
            if (applyingUser == null)
            {
                throw new NullReferenceException("The user with the specified id doesn't exist.");
            }
            var adminUser = await GetUser(adminId);
            if (adminUser.Role == "Admin")
            {
                var examUser = await _unitOfWork.Repository<ExamUser>().GetAll().Where(x => (x.ExamId == examId && x.UserId == userId)).FirstOrDefaultAsync();
                examUser.IsApproved = true;
                _unitOfWork.Repository<ExamUser>().Update(examUser);
                _unitOfWork.Complete();
            }
        }

    }
}
