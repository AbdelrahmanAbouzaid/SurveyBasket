namespace SurveyBasket.Api.Contracts.Role
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .Length(3, 100).WithMessage("Role name must be between 3 and 100 characters.");

            RuleFor(x => x.Permissions)
                .NotNull().WithMessage("Permissions are required.")
                .NotEmpty().WithMessage("Permissions cannot be empty.");

            RuleFor(x => x.Permissions)
                .Must(p => p.Distinct().Count() == p.Count())
                .WithMessage("You can not duplicate permission fore the same role.")
                .When(p => p.Permissions != null);

        }
    }
}
