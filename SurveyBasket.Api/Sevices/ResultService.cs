using SurveyBasket.Api.Contracts.Result;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Sevices
{
    public class ResultService(ApplicationDbContext context) : IResultService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollVotes = await _context.Polls
                .Where(v => v.Id == pollId)
                .Select(p => new PollVoteResponse
                (
                    p.Title,
                    p.Votes.Select(v => new VoteResponse
                    (
                        $"{v.User.FirstName}  {v.User.LastName}",
                        v.SubmittedAt,
                        v.VoteAnswers.Select(a => new QuestionAnswersResponse
                        (
                           a.Question.Content,
                           a.Answer.Content
                        ))
                    ))
                ))
                .FirstOrDefaultAsync(cancellationToken);

            return pollVotes is null
                ? Result.Failure<PollVoteResponse>(PollError.NotFound(pollId))
                : Result.Success(pollVotes);
        }
        public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls
               .AnyAsync(p => p.IsPublished && p.Id == pollId
                   && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                   && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)
                   , cancellationToken);

            if (!pollIsExists)
                return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollError.NotFound(pollId));

            var votesPerDay = await _context.Votes
                .Where(v => v.PollId == pollId)
                .GroupBy(v => v.SubmittedAt.Date)
                .Select(g => new VotesPerDayResponse(DateOnly.FromDateTime(g.Key), g.Count()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
        }

        public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls
               .AnyAsync(p => p.IsPublished && p.Id == pollId
                   && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                   && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)
                   , cancellationToken);

            if (!pollIsExists)
                return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollError.NotFound(pollId));


            var votesPerQuestion = await _context.VoteAnswers
                .Where(va => va.Vote.PollId == pollId)
                .Select(va => new VotesPerQuestionResponse(
                        va.Question.Content,
                        va.Question.VoteAnswers
                        .GroupBy(a => new {Answers= a.AnswerId, Content = a.Answer.Content })
                        .Select(g => new VotesPerAnswerResponse(g.Key.Content, g.Count()))
                    ))
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
        }
    }
}
