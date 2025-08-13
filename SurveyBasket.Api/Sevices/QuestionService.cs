using SurveyBasket.Api.Contracts.Question;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Sevices
{
    public class QuestionService(ApplicationDbContext context) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId);
            if (!pollIsExists)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollError.NotFound(pollId));

            var questions = await _context.Questions.Where(q => q.PollId == pollId).AsNoTracking()
                .ProjectToType<QuestionResponse>()
                .ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }
        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions
                .Where(q => q.PollId == pollId && q.Id == questionId)
                .Include(q => q.Answers)
                .AsNoTracking()
                .ProjectToType<QuestionResponse>()
                .FirstOrDefaultAsync(cancellationToken);

            if (question is null)
                return Result.Failure<QuestionResponse>(QuestionError.NotFound(questionId));
            return Result.Success(question);
        }
        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId);
            if (!pollIsExists)
                return Result.Failure<QuestionResponse>(PollError.NotFound(pollId));

            var questionIsExists = await _context.Questions
                .AnyAsync(q => q.PollId == pollId && q.Content == request.Content, cancellationToken);
            if (questionIsExists)
                return Result.Failure<QuestionResponse>(QuestionError.DuplicateTitle(request.Content));


            var question = request.Adapt<Question>();
            question.PollId = pollId;

            await _context.Questions.AddAsync(question, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(question.Adapt<QuestionResponse>());
        }
        public async Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var questionIsExists = await _context.Questions
                .AnyAsync(q => q.PollId == pollId 
                && q.Id != questionId 
                && q.Content == request.Content, cancellationToken);
            if (questionIsExists)
                return Result.Failure(QuestionError.DuplicateTitle(request.Content));

            var question = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.PollId == pollId && q.Id == questionId, cancellationToken);
            if (question is null)
                return Result.Failure(QuestionError.NotFound(questionId));
            question.Content = request.Content;

            //currrent answers
            var currentAnswers = question.Answers.Select(a => a.Content).ToList();
            //new answers
            var newAnswers = request.Answers.Except(currentAnswers).ToList();

            foreach (var answer in newAnswers)
            {
                question.Answers.Add(new Answer { Content = answer});
            }

            foreach (var item in question.Answers)
            {
                item.IsActive = request.Answers.Contains(item.Content);
            }
            return await _context.SaveChangesAsync(cancellationToken) > 0
                ? Result.Success()
                : Result.Failure(QuestionError.InvalidQuestionData);
        }

        public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(q => q.PollId == pollId && q.Id == questionId, cancellationToken);
            if (question is null)
                return Result.Failure(QuestionError.NotFound(questionId));
            question.IsActive = !question.IsActive;
            _context.Questions.Update(question);
            return await _context.SaveChangesAsync(cancellationToken) > 0
                ? Result.Success()
                : Result.Failure(QuestionError.InvalidQuestionData);
        }

    }
}

