
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TestOnline.Data;
using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;

namespace TestOnline.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly DataContext _dbContext;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailSender _emailSender;

        public UserController(DataContext dbContext, IConfiguration configuration, ILogger<UserController> logger, IEmailSender emailSender, IUserService userService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> Get(int id)
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
        public async Task<IActionResult> Post(UserCreateDto UserToCreate)
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
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUser(id);

            return Ok("User deleted successfully!");
        }


        
        
    }
}
