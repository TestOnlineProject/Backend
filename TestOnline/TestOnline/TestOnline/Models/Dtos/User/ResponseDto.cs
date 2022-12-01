using TestOnline.Helpers;

namespace TestOnline.Models.Dtos.User
{
    public class ResponseDto : JwtResult
    {
        public string UserId { get; set; }
    }
}
