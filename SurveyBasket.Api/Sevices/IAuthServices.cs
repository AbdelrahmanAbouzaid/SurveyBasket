using SurveyBasket.Api.Contracts.Auth;

namespace SurveyBasket.Api.Sevices
{
    public interface IAuthServices
    {
        Task<Result<AuthResponse>> GetTokenAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);

    }
}
