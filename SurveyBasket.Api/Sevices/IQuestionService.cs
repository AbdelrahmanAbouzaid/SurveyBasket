using SurveyBasket.Api.Contracts.Question;

namespace SurveyBasket.Api.Sevices
{
    public interface IQuestionService
    {
        Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<QuestionResponse>>> GetAvilableAsync(int pollId, string userId, CancellationToken cancellationToken = default);
        Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
        Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
    }
}
