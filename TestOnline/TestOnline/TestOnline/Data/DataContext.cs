//using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestOnline.Models.Entities;

namespace TestOnline.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ExamQuestion>().HasKey(eq => new
        //    {
        //        eq.ExamId,
        //        eq.QuestionId
        //    });

        //    object value = modelBuilder.Entity<ExamQuestion>().HasOne(e => e.Exam).WithMany(eq => eq.ExamQuestions).HasForeignKey(e => e.ExamId);
        //    modelBuilder.Entity<ExamQuestion>().HasOne(e => e.Question).WithMany(eq => eq.ExamQuestions).HasForeignKey(e => e.QuestionId);

        //    base.OnModelCreating(modelBuilder);
        //}

        public DbSet<Exam> Exams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
    }
}
