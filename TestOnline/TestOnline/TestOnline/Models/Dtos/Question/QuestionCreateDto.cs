using System.ComponentModel.DataAnnotations;
using TestOnline.Models.Entities;

namespace TestOnline.Models.Dtos.Question
{
    public class QuestionCreateDto
    {
        [Required, MaxLength(200)]
        public string ActualQuestion { get; set; }
        [Required]
        public string Answers { get; set; }
        [Required]
        public int CorrectAnswer { get; set; }
        public int Points { get; set; }
        public string? ImageSrc { get; set; }

    }
}
