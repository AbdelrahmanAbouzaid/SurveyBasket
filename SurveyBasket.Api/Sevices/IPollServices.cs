namespace SurveyBasket.Api.Sevices
{
    public interface IPollServices
    {
        Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default);
        Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);
    }
}
