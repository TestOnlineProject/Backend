using System.ComponentModel.DataAnnotations;

namespace TestOnline.Models.Entities
{
    public class Question
    {
        //public Question()
        //{
        //    this.Exams = new HashSet<Exam>();

        //}
        public int QuestionId { get; set; }
        [Required, MaxLength(200)]
        public string ActualQuestion { get; set; }
        [Required]
        public string Answers { get; set; }
        [Required]
        public int CorrectAnswer { get; set; }
        public int Points { get; set; }
        public string? ImageSrc { get; set; }

        //public virtual ICollection<Exam> Exams { get; set; }

    }
}
