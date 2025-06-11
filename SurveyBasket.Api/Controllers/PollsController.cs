using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Poll;
using SurveyBasket.Api.Sevices;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollServices pollServices) : ControllerBase
    {
        [HttpGet]
        [Authorize]
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
            var result = poll.Adapt<PollResponse>();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PollRequest poll, CancellationToken cancellationToken)
        {
            var result = await pollServices.AddAsync(poll.Adapt<Poll>(), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest, CancellationToken cancellationToken)
        {
            var poll = pollRequest.Adapt<Poll>();
            var flag = await pollServices.UpdateAsync(id, poll, cancellationToken);
            return flag ? Ok(flag) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var flag = await pollServices.DeleteAsync(id, cancellationToken);
            return flag ? Ok(flag) : NotFound();
        }

        [HttpGet("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
        {
            var flag = await pollServices.TogglePublishStatusAsync(id, cancellationToken);
            return flag ? Ok(flag) : NotFound();
            
        }
    }
}
