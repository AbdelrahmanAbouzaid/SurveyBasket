using SurveyBasket.Api.Contracts.Result;

namespace SurveyBasket.Api.Sevices
{
    public interface IResultService
    {
        Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default);
    }
}
