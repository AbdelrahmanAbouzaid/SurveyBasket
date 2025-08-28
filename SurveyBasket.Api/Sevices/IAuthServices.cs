using SurveyBasket.Api.Contracts.Auth;
using SurveyBasket.Api.Contracts.User;

namespace SurveyBasket.Api.Sevices
{
    public interface IAuthServices
    {
        Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);
        Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken = default);

        Task<Result> SendResetPasswordAsync(ForgetPasswordRequest request);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);

    }
}
