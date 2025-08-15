using SurveyBasket.Api.Contracts.Vote;

namespace SurveyBasket.Api.Sevices
{
    public interface IVoteService
    {
        Task<Result> AddVoteAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default);
    }
}
