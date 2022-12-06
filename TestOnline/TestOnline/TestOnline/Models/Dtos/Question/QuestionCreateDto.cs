using System.ComponentModel.DataAnnotations;
using TestOnline.Models.Entities;

namespace TestOnline.Models.Dtos.Question
{
    public class QuestionCreateDto
    {

        [Required, MaxLength(200)]
        public string ActualQuestion { get; set; }

        [Required]
        public string Option1 { get; set; }

        [Required]
        public string Option2 { get; set; }

        [Required]
        public string Option3 { get; set; }

        [Required]
        public string Option4 { get; set; }

        [Required]
        public int CorrectAnswer { get; set; }

        [Required]
        public int Points { get; set; }

        public string? ImageSrc { get; set; }

    }
}
