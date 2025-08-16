namespace SurveyBasket.Api.Contracts.Result
{
    public record VoteResponse(
        string VoterName,
        DateTime DateTime,
        IEnumerable<QuestionAnswersResponse> SelectedAnswers
        );
    
}