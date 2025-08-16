namespace SurveyBasket.Api.Contracts.Result
{
    public record VotesPerDayResponse(
        DateOnly Date,
        int VotesCount
        );
   
}
