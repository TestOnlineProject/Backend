using System.ComponentModel.DataAnnotations;

namespace TestOnline.Models.Dtos.Exam
{
    public class ExamCreateDto
    {
        public string Name { get; set; }
        [Required]
        public int NrQuestions { get; set; }
    }
}
