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
            var response = await authServices.GetTokenAsync(loginRequest, cancellationToken);
            return response.IsSuccess ? Ok(response.Value) 
                : BadRequest(response.Error);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
        {
            var response = await authServices.GetRefreshTokenAsync(refreshTokenRequest, cancellationToken);
            return response is null ? BadRequest("Invalid Login") : Ok(response);
        }
    }
}
