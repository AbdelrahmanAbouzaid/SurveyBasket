namespace SurveyBasket.Api.Contracts.Auth
{
    public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Token is required");
                
        }
    }
}
