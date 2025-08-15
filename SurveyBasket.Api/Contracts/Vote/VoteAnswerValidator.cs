namespace SurveyBasket.Api.Contracts.Vote
{
    public class VoteAnswerValidator : AbstractValidator<voteAnswerRequest>
    {
        public VoteAnswerValidator()
        {
            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .WithMessage("Option ID is required.")
                .GreaterThan(0)
                .WithMessage("Option ID must be greater than 0.");

            RuleFor(x => x.AnswerId)
                .NotEmpty()
                .WithMessage("Option ID is required.")
                .GreaterThan(0)
                .WithMessage("Option ID must be greater than 0.");
        }
    }
}
