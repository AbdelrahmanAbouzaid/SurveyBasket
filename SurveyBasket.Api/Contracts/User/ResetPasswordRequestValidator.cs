using SurveyBasket.Api.Contracts.Consts;

namespace SurveyBasket.Api.Contracts.User
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .Length(6, 100).WithMessage("Password must be between 6 and 100 characters long.")
                .Matches(Regex.Password);

        }
    }
}
