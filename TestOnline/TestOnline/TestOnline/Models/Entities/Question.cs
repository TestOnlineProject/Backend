using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestOnline.Models.Entities
{
    public class Question
    {
       
        public int QuestionId { get; set; }
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
