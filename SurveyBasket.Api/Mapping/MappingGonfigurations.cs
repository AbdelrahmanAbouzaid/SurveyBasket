using SurveyBasket.Api.Contracts.Auth;
using SurveyBasket.Api.Contracts.Question;
using SurveyBasket.Api.Contracts.User;

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

            config.NewConfig<(AppUser user, IList<string> roles), UserResponse>()
                .Map(dest => dest, src => src.user)
                .Map(dest => dest.Roles, src => src.roles);

            config.NewConfig<CreateUserRequest, AppUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.EmailConfirmed, src => true);

            config.NewConfig<UpdateUserRequest, AppUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper());
        }
    }
}
