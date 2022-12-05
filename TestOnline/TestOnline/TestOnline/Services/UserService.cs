using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using System.Diagnostics;
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
        private readonly IEmailSender _emailSender;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = _unitOfWork.Repository<User>().GetAll();
            return users.ToList();
        }

        public async Task<User> GetUser(string id)
        {
            Expression<Func<User, bool>> expression = x => x.UserId == id;
            var user = await _unitOfWork.Repository<User>().GetById(expression).FirstOrDefaultAsync();

            return user;
        }

        public async Task UpdateUser(UserDto userToUpdate)
        {
            var user = await GetUser(userToUpdate.UserId);
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

        public async Task<User?> GetByEmail(string email)
        {
            var user = await _unitOfWork.Repository<User>().GetByCondition(x => x.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public async Task CreateUser(UserCreateDto userToCreate)
        {
            var user = _mapper.Map<User>(userToCreate);

            _unitOfWork.Repository<User>().Create(user);
            _logger.LogInformation("Created user successfully!");
            _unitOfWork.Complete();
        }

        public async Task DeleteUser(string id)
        {
            var user = await GetUser(id);
            if (user == null)
            {
                throw new NullReferenceException("The user you're trying to delete doesn't exist.");
            }
            var examUsers = _unitOfWork.Repository<ExamUser>().GetByCondition(x => x.UserId == id);
            foreach (var examUser in examUsers)
            {
                examUser.UserId = null;
                _unitOfWork.Repository<ExamUser>().Update(examUser);
            }
            _unitOfWork.Repository<User>().Delete(user);

            _logger.LogInformation("Deleted user successfully!");
            _unitOfWork.Complete();
        }


       
    
        public async Task SendEmailOnRegistration(string email, string firstName)
        {
            var pathToFile = "Templates/welcome.html";
            string htmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                htmlBody = streamReader.ReadToEnd();
            }

            var myData = new[] { email, firstName};
            var content = string.Format(htmlBody, myData);

            await _emailSender.SendEmailAsync(email, "Welcome to TestOnline!", content);
            _logger.LogInformation("Sending Result email on sumbit.");
        }

        

    }
}
