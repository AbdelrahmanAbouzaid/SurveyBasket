namespace SurveyBasket.Api.Contracts.Auth
{
    public class ResendConfirmEmailRequestValidator : AbstractValidator<ResendConfirmEmailRequest>
    {
        public ResendConfirmEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
