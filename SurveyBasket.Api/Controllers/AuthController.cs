using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Auth;

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
    }
}
