namespace SurveyBasket.Api.Contracts.User
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .Length(0, 100)
                .WithMessage("First name must be between 0 and 100 characters.");
            RuleFor(x => x.LastName)
                .Length(0, 100)
                .WithMessage("Last name must be between 0 and 100 characters.");
        }
    }
}
