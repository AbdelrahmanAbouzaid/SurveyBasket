using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.User;
using SurveyBasket.Api.Extentions;

namespace SurveyBasket.Api.Controllers
{
    [Route("me")]
    [ApiController]
    [Authorize]
    public class AccountsController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var result = await userService.GetUserProfileAsync(User.GetUserId()!);
            return result.IsSuccess
                ? Ok(result.Value)
                : Problem();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateUserRequest request)
        {
            await userService.UpdateUserProfileAsync(User.GetUserId()!, request);
            return NoContent();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await userService.ChangePasswordAsync(User.GetUserId()!, request);
            
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
