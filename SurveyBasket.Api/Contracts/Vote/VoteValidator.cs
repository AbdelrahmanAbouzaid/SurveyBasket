namespace SurveyBasket.Api.Contracts.Vote
{
    public class VoteValidator : AbstractValidator<VoteRequest>
    {
        public VoteValidator()
        {
            RuleFor(x => x.Answers)
                .NotEmpty()
                .WithMessage("Option ID is required.");
            RuleForEach(x => x.Answers)
                .SetInheritanceValidator(x => x.Add(new VoteAnswerValidator()));
        }
    }
}
