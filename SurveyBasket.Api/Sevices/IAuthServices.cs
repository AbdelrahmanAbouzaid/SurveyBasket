using SurveyBasket.Api.Contracts.Auth;

namespace SurveyBasket.Api.Sevices
{
    public interface IAuthServices
    {
        Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

    }
}
