namespace SurveyBasket.Api.Contracts.User
{
    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Required Email")
            .EmailAddress();
        }
    }
}
