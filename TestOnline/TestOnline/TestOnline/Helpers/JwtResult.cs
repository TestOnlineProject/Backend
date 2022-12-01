﻿namespace TestOnline.Helpers
{
    public class JwtResult
    {
        public string Token { get; set; } // The generated JWT token
        public bool Succedded { get; set; } // True or Fales based on the Authorization Status
        public List<string> Errors { get; set; } // Custom Error Message
    }
}
