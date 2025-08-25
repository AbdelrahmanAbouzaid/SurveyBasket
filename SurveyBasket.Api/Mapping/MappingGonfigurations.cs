using SurveyBasket.Api.Contracts.Auth;
using SurveyBasket.Api.Contracts.Question;

namespace SurveyBasket.Api.Mapping
{
    public class MappingGonfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<QuestionRequest, Question>()
                .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));
                
            config.NewConfig<RegisterRequest, AppUser>()
                .Map(dest => dest.UserName, src => src.Email);
        }
    }
}
