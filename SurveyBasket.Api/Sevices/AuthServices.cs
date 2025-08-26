using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Auth;
using SurveyBasket.Api.Helpers;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.Api.Sevices
{
    public class AuthServices(
        UserManager<AppUser> userManager,
        ILogger<AuthServices> logger,
        IJwtProvider jwtProvider,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor) : IAuthServices
    {
        private readonly int _tokenExpiryDays = 15;
        private readonly ILogger<AuthServices> logger = logger;
        private readonly IEmailService emailService = emailService;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
        {
            var userId = jwtProvider.ValidateToken(refreshTokenRequest.Token);
            if (userId is null)
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenRequest.RefreshToken && x.IsActive);
            if (userRefreshToken is null) return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

            userRefreshToken.RevokedOn = DateTime.UtcNow;


            var (newToken, exprieIn) = jwtProvider.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_tokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiry
            });

            await userManager.UpdateAsync(user);

            var response = new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                newToken,
                exprieIn,
                newRefreshToken,
                refreshTokenExpiry
                );

            return Result.Success(response);
        }
        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var existingUser = await userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (existingUser)
                return Result.Failure(UserError.DuplicateEmail);

            var user = request.Adapt<AppUser>();

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            logger.LogInformation("Email confirmation code for {email} is {code}", user.Email, code);

            await SendEmailConfirmationAsync(user, code);
            
            return Result.Success();
        }
        public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(loginRequest.email);
            if (user is null)
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

            var flag = await userManager.CheckPasswordAsync(user, loginRequest.password);
            if (!flag)
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

            var (token, exprieIn) = jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_tokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiry
            });

            await userManager.UpdateAsync(user);

            var result = new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Token: token,
                exprieIn,
                refreshToken,
                refreshTokenExpiry
                );

            return Result.Success(result);
        }


        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return Result.Failure(UserError.InvalidCredentials);

            if (user.EmailConfirmed)
                return Result.Failure(UserError.DuplicateConfirmation);

            var code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            }
            catch (FormatException)
            {
                return Result.Failure(UserError.InvalidCode);
            }

            var result = await userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
            return Result.Success();
        }

        public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserError.DuplicateConfirmation);

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            logger.LogInformation("Email confirmation code for {email} is {code}", user.Email, code);

            await SendEmailConfirmationAsync(user, code);

            return Result.Success();

        }
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private async Task SendEmailConfirmationAsync(AppUser user, string code)
        {
            var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
            {
                {"{{name}}", $"{user.FirstName}" },
                {"{{action_url}}", $"{origin}/auth/confirm-email?userid={user.Id}&code={code}" },

            });
            await emailService.SendEmailAsync(user.Email!, emailBody);
        }
    }
}
