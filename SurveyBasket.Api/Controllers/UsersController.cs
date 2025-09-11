using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.User;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.ReadUsers)]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await userService.GetAllUsersAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        [HasPermission(Permissions.ReadUsers)]
        public async Task<IActionResult> GetUsers([FromRoute] string userId)
        {
            var result = await userService.GetAsync(userId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost]
        [HasPermission(Permissions.CreateUsers)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await userService.CreateAsync(request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetUsers), new { userId = result.Value!.Id }, result.Value) : result.ToProblem();
        }

        [HttpPut("{userId}")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UpdateUser([FromRoute] string userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await userService.UpdateAsync(userId, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{userId}/toggle-status")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string userId)
        {
            var result = await userService.ToggleStatusAsync(userId);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{userId}/unlock")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UnLock([FromRoute] string userId)
        {
            var result = await userService.UnLockAsync(userId);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
