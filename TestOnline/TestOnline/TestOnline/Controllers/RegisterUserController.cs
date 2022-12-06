using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestOnline.Data.UnitOfWork;
using TestOnline.Models.Dtos.User;
using TestOnline.Services.IService;

namespace TestOnline.Controllers
{
    [ApiController]
    [Route("user")]
    public class RegisterUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string _jwtConfiguration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public RegisterUserController(UserManager<IdentityUser> userManager, IConfiguration configuration, IUnitOfWork unitOfWork, IUserService userService)
        {
            _jwtConfiguration = configuration.GetValue<string>("JWTConfig:Secret");
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userService.GetByEmail(user.Email);

                if (existingUser == null)
                {
                    var newUser = new IdentityUser()
                    {
                        Email = user.Email,
                        UserName = user.UserName,

                    };
                    var Created = await _userManager.CreateAsync(newUser, user.Password);

                    if (Created.Succeeded)
                    {
                        _unitOfWork.Repository<Models.Entities.User>().Create(new Models.Entities.User
                        {
                            UserId = newUser.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            UserName = user.UserName,
                            Location = user.Location,
                            BirthDate = user.BirthDate,

                        });

                        _unitOfWork.Complete();
                        var jwtToken = GenerateJWT(newUser);

                        _userService.SendEmailOnRegistration(user.Email, user.FirstName);
                        return Ok(new ResponseDto
                        {
                            Token = jwtToken,
                            Succedded = true,
                        });
                    }
                    else
                    {
                        return BadRequest(new ResponseDto()
                        {
                            Errors = Created.Errors.Select(error => error.Description).ToList(),
                            Succedded = false
                        }); ;
                    }
                }
                else
                {
                    return BadRequest(new ResponseDto()
                    {
                        Errors = new List<string>
                        {
                            "A user with this email already Exists"
                        },
                        Succedded = false
                    });
                }

            }

            return BadRequest(new ResponseDto()
            {
                Errors = new List<string>
                {
                    "Invalid Request"
                },
                Succedded = false
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(user.UserName);

                if (existingUser == null)
                {
                    return BadRequest(new ResponseDto()
                    {
                        Errors = new List<string>
                        {
                            "This user does not exist!"
                        },
                        Succedded = false
                    });
                }

                var validUser = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (!validUser)
                {
                    return BadRequest(new ResponseDto()
                    {
                        Errors = new List<string>
                        {
                            "Incorrect Password!"
                        },
                        Succedded = false
                    });
                }

                var jwtToken = GenerateJWT(existingUser);

                return Ok(new ResponseDto()
                {
                    Token = jwtToken,
                    Succedded = true
                });
            }

            return BadRequest(new ResponseDto()
            {
                Errors = new List<string>
                {
                    "Invalid Request"
                },
                Succedded = false
            });
        }

        private string GenerateJWT(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfiguration);
            var myUser = _unitOfWork.Repository<Models.Entities.User>().GetByCondition(x => x.UserId == user.Id).FirstOrDefault();
            var role = "User";
            if (myUser != null)
            {
                role = myUser.Role;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = "http://localhost:43000",
                Audience = "http://localhost:43000"
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}

