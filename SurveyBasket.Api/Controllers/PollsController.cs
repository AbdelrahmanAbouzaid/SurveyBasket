using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Poll;
using SurveyBasket.Api.Sevices;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PollsController(IPollServices pollServices) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var polls = await pollServices.GetAllAsync();
            var response = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var poll = await pollServices.GetAsync(id);
            var result = poll.Value.Adapt<PollResponse>();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PollRequest poll, CancellationToken cancellationToken)
        {
            var result = await pollServices.AddAsync(poll, cancellationToken);
            return Ok(result.Value.Adapt<PollResponse>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest, CancellationToken cancellationToken)
        {
            var result = await pollServices.UpdateAsync(id, pollRequest, cancellationToken);
            return result.IsSuccess ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await pollServices.DeleteAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : NotFound();
        }

        [HttpGet("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await pollServices.TogglePublishStatusAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : NotFound();

        }
    }
}
