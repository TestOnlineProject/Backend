using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TestOnline.Models.Entities
{
    public class Exam
    {
        public int ExamId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int NrQuestions { get; set; }

        public int TotalPoints { get; set; } = 0;
    }
}
