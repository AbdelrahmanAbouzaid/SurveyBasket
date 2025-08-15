using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Question;
using SurveyBasket.Api.Errors;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        [HttpGet]
        public async Task<IActionResult> GetQuestions([FromRoute] int pollId, CancellationToken cancellationToken = default)
        {
            var result = await _questionService.GetAllAsync(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var result = await _questionService.GetAsync(pollId, id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromRoute] int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _questionService.AddAsync(pollId, request, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _questionService.UpdateAsync(pollId, id, request, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}/toggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
