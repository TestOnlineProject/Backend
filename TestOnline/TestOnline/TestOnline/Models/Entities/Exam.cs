using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TestOnline.Models.Entities
{
    public class Exam
    {
        //public Exam()
        //{
        //    this.Questions = new HashSet<Question>();
        //}
        public int ExamId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int NrQuestions { get; set; }
        public int TotalPoints { get; set; } = 0;

        //public virtual ICollection<Question> Questions { get; set; }
    }
}
