using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Contracts.Question;
using SurveyBasket.Api.Contracts.Vote;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Sevices
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result> AddVoteAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
        {
            var hasVote = await _context.Votes
                .AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

            if (hasVote)
                return Result.Failure(VoteError.DuplicateVote);

            var pollIsExists = await _context.Polls
                .AnyAsync(p => p.IsPublished && p.Id == pollId
                    && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                    && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));

            if (!pollIsExists)
                return Result.Failure(PollError.NotFound(pollId));

            var availableQuestions = await _context.Questions
                .Where(q => q.PollId == pollId && q.IsActive)
                .Select(q => q.Id)
                .ToListAsync(cancellationToken);

            if(!request.Answers.Select(a => a.QuestionId).SequenceEqual(availableQuestions))
                return Result.Failure(VoteError.InvalidOperation);

            var vote = new Vote
            {
                PollId = pollId,
                UserId = userId,
                VoteAnswers = request.Answers.Adapt<List<VoteAnswer>>()
            };
            await _context.Votes.AddAsync(vote, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
