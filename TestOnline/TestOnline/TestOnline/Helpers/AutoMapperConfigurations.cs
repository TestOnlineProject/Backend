using AutoMapper;
using TestOnline.Models.Dtos.Exam;
using TestOnline.Models.Dtos.Question;
using TestOnline.Models.Dtos.User;
using TestOnline.Models.Entities;

namespace TestOnline.Helpers
{
    public class AutoMapperConfigurations : Profile
    {
        public AutoMapperConfigurations()
        {
            CreateMap<Exam, ExamDto>().ReverseMap();
            CreateMap<Exam, ExamCreateDto>().ReverseMap();

            CreateMap<Question, QuestionDto>().ReverseMap();
            CreateMap<Question, QuestionCreateDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserCreateDto>().ReverseMap();
        }
    }

}
