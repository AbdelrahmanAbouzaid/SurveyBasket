namespace SurveyBasket.Api.Contracts.User
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
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
