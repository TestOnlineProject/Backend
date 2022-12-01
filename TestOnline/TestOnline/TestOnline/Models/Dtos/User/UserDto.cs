using System.ComponentModel.DataAnnotations;

namespace TestOnline.Models.Dtos.User
{
    public class UserDto
    {
        public string UserId { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]

        public string LastName { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string Location { get; set; }
        public string Role { get; set; } = "User";
    }
}
