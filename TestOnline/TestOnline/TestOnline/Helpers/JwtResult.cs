namespace TestOnline.Helpers
{
    public class JwtResult
    {
        public string Token { get; set; } 
        public bool Succedded { get; set; } 
        public List<string> Errors { get; set; } 
    }
}
