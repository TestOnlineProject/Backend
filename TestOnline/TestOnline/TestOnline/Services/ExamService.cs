using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ExamService> logger, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<List<Exam>> GetAllExams()
        {
            var exams = _unitOfWork.Repository<Exam>().GetAll();
            return exams.ToList();
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


        public async Task CreateExam(int nrOfQuestions, string name)
        {
            Exam exam = new Exam { NrQuestions = nrOfQuestions, Name = name, TotalPoints = 0 };
            Random rand = new Random();

            #region Implementimi i Random ne nje menyre tjeter
            //var questions = new List<Question>();
            //int numberOfQuestions = _unitOfWork.Repository<Question>().GetAll().Count();
            //for (int i = 0; i < nrOfQuestions; i++)
            //{
            //    int skip = rand.Next(0, numberOfQuestions);
            //    questions.Add(_unitOfWork.Repository<Question>().GetAll().Skip(skip).Take(1).First());
            //}
            #endregion

            var questions = _unitOfWork.Repository<Question>().GetAll().OrderBy(q => Guid.NewGuid()).Take(nrOfQuestions);

            foreach (var question in questions)
            {
                exam.TotalPoints += question.Points;

                ExamQuestion examQuestion = new ExamQuestion
                {
                    Question = question,
                    QuestionId = question.QuestionId,

                    Exam = exam,
                    ExamId = exam.ExamId
                };

                _unitOfWork.Repository<ExamQuestion>().Create(examQuestion);
            }
            _unitOfWork.Repository<Exam>().Create(exam);
            _unitOfWork.Complete();
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

        public async Task<List<Question>> StartExam(string userId, int examId)
        {
            var examUser = await _unitOfWork.Repository<ExamUser>().GetByCondition(x => (x.ExamId == examId && x.UserId == userId)).FirstOrDefaultAsync();
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
        public async Task<double> SubmitExam(int examId, List<int> answers)
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
            //var pathToFile = "Templates/order_confirmation.html";

            //string htmlBody = "";
            //using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            //{
            //    htmlBody = streamReader.ReadToEnd();
            //}

            //double totalPrice = 0;
            //shoppingCardItems.ForEach(x => totalPrice += x.ProductPrice);

            //var orderIds = orders.Select(x => x.OrderId).ToList();

            ////var totalPrice = shoppingCardItems.Select(x => x.ProductPrice).Sum();
            //var orderConfirmationDto = new OrderConirmationDto
            //{
            //    UserName = "LifeUser",
            //    OrderDate = DateTime.Now,
            //    Price = totalPrice,
            //    OrderId = string.Join(",", orderIds)
            //};

            //var myData = new[] { "LifeUser", DateTime.Now.ToString(), totalPrice.ToString(), string.Join(",", orderIds) };

            //var content = string.Format(htmlBody, myData);

            //await _emailSender.SendEmailAsync(addressDetails.Email, "OrderConfirmation", content);
            var content = $"Your exam was submitted successfully.\n Percentage:{percentage}, Grade:{grade}";
            await _emailSender.SendEmailAsync("jeton.sllamniku@life.gjirafa.com", "Exam Result!", content);
            _logger.LogInformation("Sending Result email on sumbit.");
            return percentage;
        }


        //public async Task<PagedInfo<Exam>> ExamsListView(string search, int page, int pageSize)
        //{
        //    Expression<Func<Exam, bool>> condition = x => x.Name.Contains(search);

        //    //var exams1 = _unitOfWork.Repository<Exam>()
        //    //                                             .GetByConditionPaginated(condition, x => x.ExamId, page, pageSize, false);

        //    var exams = _unitOfWork.Repository<Exam>()
        //                                                 .GetAll().WhereIf(!string.IsNullOrEmpty(search), condition);

        //    var count = await exams.CountAsync();

        //    var examsPaged = new PagedInfo<Exam>()
        //    {
        //        TotalCount = count,
        //        Page = page,
        //        PageSize = pageSize,
        //        Data = await exams
        //                    .Skip((page - 1) * pageSize)
        //                    .Take(pageSize).ToListAsync()
        //    };

        //    return examsPaged;
        //}
    }
}
