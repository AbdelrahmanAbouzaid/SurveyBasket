namespace SurveyBasket.Api.Contracts.Question
{
    public class QuestionValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionValidator()
        {
            RuleFor(q => q.Content)
                .NotEmpty()
                .WithMessage("Question text is required.")
                .MaximumLength(1000)
                .WithMessage("Question text must not exceed 500 characters.");

            RuleFor(q => q.Answers).NotNull();

            RuleFor(q => q.Answers)
                .Must(answers => answers != null && answers.Count > 1)
                .WithMessage("At least two answer is required.")
                .When(q => q.Answers != null);

            RuleFor(q => q.Answers)
                .Must(answer => answer.Distinct().Count() == answer.Count())
                .WithMessage("Duplicated answer.")
                .When(q => q.Answers != null);

        }
    }
}
