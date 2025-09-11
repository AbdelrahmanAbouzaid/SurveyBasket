namespace SurveyBasket.Api.Contracts.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .Length(0, 100)
                .WithMessage("First name must be between 0 and 100 characters.");

            RuleFor(x => x.LastName)
                .Length(0, 100)
                .WithMessage("Last name must be between 0 and 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(Regex.Password);

            RuleFor(x => x.Roles)
                .NotNull().WithMessage("Roles are required.")
                .NotEmpty().WithMessage("Roles cannot be empty.");

            RuleFor(x => x.Roles)
                .Must(r => r.Distinct().Count() == r.Count())
                .WithMessage("You can not duplicate role.")
                .When(r => r.Roles != null);


        }
    }
}
