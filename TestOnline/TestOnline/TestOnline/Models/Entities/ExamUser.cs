using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestOnline.Models.Entities
{
    public class ExamUser
    {
        [Key]
        public int Id { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime? StartTime { get; set; } // The time when the user started taking the exam

        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public User User { get; set; }


        [ForeignKey("ExamId")]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
