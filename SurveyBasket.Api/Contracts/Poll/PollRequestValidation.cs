namespace SurveyBasket.Api.Contracts.Poll
{
    public class PollRequestValidation : AbstractValidator<PollRequest>
    {
        public PollRequestValidation()
        {
            
            RuleFor(p => p.Title).NotEmpty().Length(3, 100);
            RuleFor(p => p.Summary).NotEmpty().Length(3, 1000);
            RuleFor(p => p.StartsAt).NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

            RuleFor(p => p).Must(HasValidDate)
                .WithName(nameof(PollRequest.EndsAt))
                .WithMessage("{PropertyName} Must be greater than or equal Start date");
                
        }

        private bool HasValidDate(PollRequest pollRequest)
        {
            return pollRequest.EndsAt >= pollRequest.StartsAt;
        }
    }
}
