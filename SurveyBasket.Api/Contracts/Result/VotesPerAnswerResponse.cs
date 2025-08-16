namespace SurveyBasket.Api.Contracts.Result
{
    public record VotesPerAnswerResponse(
            string AnswerContent,
            int VotesCount
        );
}