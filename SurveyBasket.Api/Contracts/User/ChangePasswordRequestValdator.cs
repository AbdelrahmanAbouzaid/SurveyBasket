using SurveyBasket.Api.Contracts.Const;

namespace SurveyBasket.Api.Contracts.User
{
    public class ChangePasswordRequestValdator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValdator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Current password is required.")
                .MinimumLength(6).WithMessage("Current password must be at least 6 characters long.")
                .NotEqual(x => x.CurrentPassword).WithMessage("New Password must not be equal to Current Password")
                .Matches(Regex.Password);
        }
    }
}
