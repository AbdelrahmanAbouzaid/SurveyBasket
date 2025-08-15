namespace SurveyBasket.Api.Contracts.Vote;
    public record voteAnswerRequest(
        int QuestionId,
        int AnswerId
        );
    