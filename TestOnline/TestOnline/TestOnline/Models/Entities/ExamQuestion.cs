using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestOnline.Models.Entities
{
    // "Exam has questions and question can be in a lot of exams" 
    public class ExamQuestion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("QuestionId")]
        public int QuestionId { get; set; }

        public Question Question { get; set; }


        [ForeignKey("ExamId")]
        public int ExamId { get; set; }

        public Exam Exam { get; set; }

    }
}
