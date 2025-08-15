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
            var result = await pollServices.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var result = await pollServices.GetCurrentAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await pollServices.GetAsync(id);
            return result.IsSuccess ?
                Ok(result.Value) :
                result.ToProblem();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PollRequest poll, CancellationToken cancellationToken)
        {
            var result = await pollServices.AddAsync(poll, cancellationToken);
            return result.IsSuccess ? 
                CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value) : 
                result.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest, CancellationToken cancellationToken)
        {
            var result = await pollServices.UpdateAsync(id, pollRequest, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await pollServices.DeleteAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpGet("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await pollServices.TogglePublishStatusAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();

        }
    }
}
