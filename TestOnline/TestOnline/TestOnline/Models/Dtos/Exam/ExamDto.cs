using System.ComponentModel.DataAnnotations;
using TestOnline.Models.Entities;

namespace TestOnline.Models.Dtos.Exam
{
    public class ExamDto
    {
        public int ExamId { get; set; }
        public string Name { get; set; }
       
    }
}
