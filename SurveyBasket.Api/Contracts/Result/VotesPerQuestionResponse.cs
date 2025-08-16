namespace SurveyBasket.Api.Contracts.Result
{
    public record VotesPerQuestionResponse(
        string QuestionContent,
        IEnumerable<VotesPerAnswerResponse> SelectedAnswers
        );
}
