using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Sevices
{
    public class PollServices(ApplicationDbContext context) : IPollServices
    {


        public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => await context.Polls
            .AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);
        public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default)
            => await context.Polls
            .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                    && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);
        public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await context.Polls.FindAsync(id, cancellationToken);
            return poll is null ?
                Result.Failure<PollResponse>(PollError.NotFound(id))
                : Result.Success(poll.Adapt<PollResponse>());
        }

        public async Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
        {
            var isExistTitle = await context.Polls
                .AnyAsync(p => p.Title == pollRequest.Title, cancellationToken);

            if (isExistTitle) 
                return Result.Failure<PollResponse>(PollError.DuplicateTitle(pollRequest.Title));

            var poll = pollRequest.Adapt<Poll>();
            await context.AddAsync(pollRequest.Adapt<Poll>(), cancellationToken);
            var count = await context.SaveChangesAsync(cancellationToken);
            return count > 0
                ? Result.Success(pollRequest.Adapt<PollResponse>())
                : Result.Failure<PollResponse>(PollError.InvalidPollData);
        }
        public async Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken = default)
        {
            var isExistTitle = await context.Polls
                .AnyAsync(p => p.Title == pollRequest.Title, cancellationToken);
            if (isExistTitle) 
                return Result.Failure<PollResponse>(PollError.DuplicateTitle(pollRequest.Title));

            var currentPoll = await context.Polls.FindAsync(id, cancellationToken);
            if (currentPoll is null)
                return Result.Failure(PollError.NotFound(id));


            currentPoll.Title = pollRequest.Title;
            currentPoll.Summary = pollRequest.Summary;
            currentPoll.StartsAt = pollRequest.StartsAt;
            currentPoll.EndsAt = pollRequest.EndsAt;

            var count = await context.SaveChangesAsync(cancellationToken);
            return count > 0
                ? Result.Success()
                : Result.Failure(PollError.InvalidPollData);
        }
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var currentPoll = await context.Polls.FindAsync(id, cancellationToken);
            if (currentPoll is null)
                return Result.Failure(PollError.NotFound(id));

            context.Remove(currentPoll);
            var count = await context.SaveChangesAsync(cancellationToken);
            return count > 0
                ? Result.Success()
                : Result.Failure(PollError.InvalidPollData);

        }

        public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            var currentPoll = await context.Polls.FindAsync(id, cancellationToken);
            if (currentPoll is null)
                return Result.Failure(PollError.NotFound(id));


            currentPoll.IsPublished = !currentPoll.IsPublished;

            var count = await context.SaveChangesAsync(cancellationToken);
            return count > 0
                ? Result.Success()
                : Result.Failure(PollError.InvalidPollData);
        }

       
    }
}
