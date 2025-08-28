using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Auth;
using SurveyBasket.Api.Contracts.User;

namespace SurveyBasket.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthServices authServices) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var result = await authServices.GetTokenAsync(loginRequest, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) 
                : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
        {
            var result = await authServices.GetRefreshTokenAsync(refreshTokenRequest, cancellationToken);
            return result.IsSuccess ?  Ok(result) : result.ToProblem();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await authServices.RegisterAsync(request, cancellationToken);
            return result.IsSuccess ?  Ok() : result.ToProblem();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await authServices.ConfirmEmailAsync(request, cancellationToken);
            return result.IsSuccess ?  Ok() : result.ToProblem();
        }

        [HttpPost("resend-confirm-email")]
        public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await authServices.ResendConfirmEmailAsync(request, cancellationToken);
            return result.IsSuccess ?  Ok() : result.ToProblem();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await authServices.SendResetPasswordAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
             
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await authServices.ResetPasswordAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
             
        }
    }
}
