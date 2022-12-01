
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TestOnline.Data;
using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;
using TestOnline.Services;
using TestOnline.Services.IService;

namespace TestOnline.Controllers
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailSender _emailSender;

        public UserController(IConfiguration configuration, ILogger<UserController> logger, IUserService userService, IEmailSender emailSender)
        {
            _logger = logger;
            _userService = userService;
            _emailSender = emailSender;
        }

        [HttpGet("isAuthenticated")]
        [AllowAnonymous]
        public async Task<bool> IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();  
            }

            return Ok(user);
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();

            return Ok(users);
        }

        //[HttpGet("UsersListView")]
        //public async Task<IActionResult> UsersListView(string? search, int page = 1, int pageSize = 10)
        //{
        //    var users = await _userService.UsersListView(search, page, pageSize);

        //    return Ok(users);
        //}

        [HttpPost("PostUser")]
        public async Task<IActionResult> CreateUser(UserCreateDto UserToCreate)
        {
            await _userService.CreateUser(UserToCreate);

            return Ok("User created successfully!");
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> Update(UserDto UserToUpdate)
        {
            await _userService.UpdateUser(UserToUpdate);

            return Ok("User updated successfully!");
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUser(id);

            return Ok("User deleted successfully!");
        }


        [HttpPost("RequestToTakeTheExam")]
        public async Task<IActionResult> RequestToTakeTheExam(string userId, int examId)
        {
            await _userService.RequestToTakeTheExam(userId, examId);

            return Ok("Request for taking the exam is created successfully!");
        }

        [HttpPut("ApproveExam")]
        public async Task<IActionResult> ApproveExam(string userId, int examId, string adminId)
        {
            await _userService.ApproveExam(userId, examId, adminId);
            return Ok($"The exam request for user with Id:{userId} was approved successfully!");
        }
    }
}
