using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Org.BouncyCastle.Crypto;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TestOnline.Data.UnitOfWork;
using TestOnline.Models.Dtos.Exam;
using TestOnline.Models.Entities;
using TestOnline.Services.IService;

namespace TestOnline.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExamService> _logger;

        public ExamService(IUnitOfWork unitOfWork, ILogger<ExamService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Exam> GetExam(int id)
        {
            Expression<Func<Exam, bool>> expression = x => x.ExamId == id;
            var exam = await _unitOfWork.Repository<Exam>().GetById(expression).FirstOrDefaultAsync();
            return exam;
        }

        public async Task<List<Question>> GetExamQuestions(int id)
        {
            var exam = await GetExam(id);
            if (exam == null)
            {
                throw new NullReferenceException("The exam with the specified id doesn't exist.");
            }
            var questionIds = _unitOfWork.Repository<ExamQuestion>().GetByCondition(x => x.ExamId == id).Select(x => x.QuestionId).ToList();
            var questions = _unitOfWork.Repository<Question>().GetByCondition(x => questionIds.Contains(x.QuestionId)).ToList();
            return questions;
        }

        public async Task<List<Exam>> GetAllExams()
        {
            var exams = _unitOfWork.Repository<Exam>().GetAll();
            return exams.ToList();
        }


        public async Task CreateExam(int nrOfQuestions, string name)
        {
            if (nrOfQuestions <= 0)
            {
                throw new ArgumentException("The number of questions must be positive!");
            }
            Exam exam = new Exam { NrQuestions = nrOfQuestions, Name = name, TotalPoints = 0 };

            #region Implementimi i Random ne nje menyre tjeter
            //Random rand = new Random();
            //var questions = new List<Question>();
            //int numberOfQuestions = _unitOfWork.Repository<Question>().GetAll().Count();
            //for (int i = 0; i < nrOfQuestions; i++)
            //{
            //    int skip = rand.Next(0, numberOfQuestions);
            //    questions.Add(_unitOfWork.Repository<Question>().GetAll().Skip(skip).Take(1).First());
            //}
            #endregion

            var questions = _unitOfWork.Repository<Question>().GetAll().OrderBy(q => Guid.NewGuid()).Take(nrOfQuestions);

            foreach (var q in questions)
            {
                exam.TotalPoints += q.Points;

                ExamQuestion examQuestion = new ExamQuestion
                {
                    Question = q,
                    QuestionId = q.QuestionId,

                    Exam = exam,
                    ExamId = exam.ExamId
                };

                _unitOfWork.Repository<ExamQuestion>().Create(examQuestion);
            }
            _unitOfWork.Repository<Exam>().Create(exam);
            _unitOfWork.Complete();
            _logger.LogInformation("Created Exam successfully!");
        }


        public async Task UpdateExam(ExamDto examToUpdate)
        {
            var exam = await GetExam(examToUpdate.ExamId);
            if (exam == null)
            {
                throw new NullReferenceException("The exam you're trying to update doesn't exist!");
            }
            exam.Name = examToUpdate.Name;

            _unitOfWork.Repository<Exam>().Update(exam);

            _unitOfWork.Complete();
        }

        public async Task DeleteExam(int id)
        {
            var exam = await GetExam(id);
            if (exam == null)
            {
                throw new NullReferenceException("The exam you're trying to delete doesn't exist.");
            }

            // When deleting an exam, also delete all ocurrences of that exam from the ExamQuestion table
            var examQuestions = _unitOfWork.Repository<ExamQuestion>().GetByCondition(x => x.ExamId == id).ToList();
            foreach (var examQuestion in examQuestions)
            {
                _unitOfWork.Repository<ExamQuestion>().Delete(examQuestion);
            }
            _unitOfWork.Repository<Exam>().Delete(exam);
            _unitOfWork.Complete();
        }

        public async Task<ExamUser> GetExamUser(string userId, int examId)
        {
            var examUser = await _unitOfWork.Repository<ExamUser>().GetByCondition(x => (x.ExamId == examId && x.UserId == userId)).FirstOrDefaultAsync();
            return examUser;
        }

        public async Task<User> GetUser(string id)
        {
            var user = await _unitOfWork.Repository<User>().GetById(x => x.UserId == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task RequestToTakeTheExam(string userId, int examId)
        {
            var examUser = await GetExamUser(userId, examId);
            if (examUser is not null)
            {
                throw new Exception("You're not allowed to retake the exam!");
            }
            var user = await GetUser(userId);
            if (user == null)
            {
                throw new NullReferenceException("The user who requested to take the exam is not registered!");
            }
            var exam = await _unitOfWork.Repository<Exam>().GetById(x => x.ExamId == examId).FirstOrDefaultAsync();
            if (exam == null)
            {
                throw new NullReferenceException("The exam specified doesn't exist.");
            }

            examUser = new ExamUser
            {
                User = user,
                UserId = userId,

                Exam = exam,
                ExamId = examId
            };
            _unitOfWork.Repository<ExamUser>().Create(examUser);
            _unitOfWork.Complete();
        }

        public async Task ApproveExam(string userId, int examId)
        {
            var applyingUser = await GetUser(userId);
            if (applyingUser == null)
            {
                throw new NullReferenceException("The user with the specified id doesn't exist.");
            }

            var examUser = await GetExamUser(userId, examId);
            examUser.IsApproved = true;
            _unitOfWork.Repository<ExamUser>().Update(examUser);
            _unitOfWork.Complete();

        }

        public async Task<List<Question>> StartExam(string userId, int examId)
        {
            var examUser = await _unitOfWork.Repository<ExamUser>().GetByCondition(x => (x.ExamId == examId && x.UserId == userId)).FirstOrDefaultAsync();
            if (examUser == null)
            {
                throw new NullReferenceException("You should make a request to take the exam first.");
            }
            if (!examUser.IsApproved)
            {
                throw new AccessViolationException("The user can't take the exam since it's not approved!");
            }
            examUser.StartTime = DateTime.Now;
            _unitOfWork.Repository<ExamUser>().Update(examUser);
            _unitOfWork.Complete();
            var questions = await GetExamQuestions(examId);
            return questions;
        }

        public async Task<double> SubmitExam(int examId, string userId, List<int> answers)
        {
            double points = 0;
            var exam = await GetExam(examId);
            var questions = await GetExamQuestions(examId);
            for (int i = 0; i < questions.Count; i++)
            {
                if (questions[i].CorrectAnswer == answers[i])
                {
                    points += questions[i].Points;
                }
            }
            double percentage = Math.Round(points / exam.TotalPoints * 100, 2);
            int grade = 5;
            if (percentage >= 50 && percentage < 60)
            {
                grade = 6;
            }
            else if (percentage >= 60 && percentage < 70)
            {
                grade = 7;
            }
            else if (percentage >= 70 && percentage < 80)
            {
                grade = 8;
            }
            else if (percentage >= 80 && percentage < 90)
            {
                grade = 9;
            }
            else if (percentage >= 90 && percentage <= 100)
            {
                grade = 10;
            }
            var pathToFile = "Templates/examResult.html";

            string htmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                htmlBody = streamReader.ReadToEnd();
            }
            var user = await _unitOfWork.Repository<User>().GetById(x => x.UserId == userId).FirstOrDefaultAsync();

            if (user is null)
            {
                throw new NullReferenceException("There is no user with the given Id.");
            }
            var myData = new string[] { user.FirstName, DateTime.Now.ToString(), grade.ToString(), percentage.ToString() };

            var content = string.Format(htmlBody, myData);

            // Unkomento keto me poshte per te derguar email, ndoshta mund te kete ndonje error te vogel pasi qe nuk kam mundur ta testoj sepse nuk ishte e mundur te dergojme email perveqse nga gjirafa.
            //await _emailSender.SendEmailAsync(user.Email, "Exam Result!", content);
            //_logger.LogInformation("Sending Result email on submit.");
            return percentage;
        }
    }
}
