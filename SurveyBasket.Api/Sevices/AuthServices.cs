using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Auth;
using SurveyBasket.Api.Contracts.Consts;
using SurveyBasket.Api.Contracts.User;
using SurveyBasket.Api.Helpers;
using SurveyBasket.Api.Persistence;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.Api.Sevices
{
    public class AuthServices(
        UserManager<AppUser> userManager,
        ILogger<AuthServices> logger,
        IJwtProvider jwtProvider,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor,
        SignInManager<AppUser> signinManager,
        ApplicationDbContext context) : IAuthServices
    {
        private readonly int _tokenExpiryDays = 15;
        private readonly ILogger<AuthServices> logger = logger;
        private readonly IEmailService emailService = emailService;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        private readonly SignInManager<AppUser> signinManager = signinManager;
        private readonly ApplicationDbContext context = context;

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

            if(user.IsDisabled)
                return Result.Failure<AuthResponse>(UserError.UserDisabled);

            //var flag = await userManager.CheckPasswordAsync(user, loginRequest.password);
            //if (!flag)
            //    return Result.Failure<AuthResponse>(UserError.InvalidCredentials);
            var result = await signinManager.CheckPasswordSignInAsync(user, loginRequest.password, true);
            if (!result.Succeeded)
            {
                var error = result.IsNotAllowed ? UserError.EmailNotConfirmed 
                    : result.IsLockedOut 
                    ? UserError.UserDisabled
                    : UserError.InvalidCredentials;
                return Result.Failure<AuthResponse>(error);
            }

            var (roles, permissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (token, exprieIn) = jwtProvider.GenerateToken(user, roles, permissions);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_tokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiry
            });

            await userManager.UpdateAsync(user);

            var response = new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Token: token,
                exprieIn,
                refreshToken,
                refreshTokenExpiry
                );

            return Result.Success(response);
        }
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
            var (roles, permissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (newToken, exprieIn) = jwtProvider.GenerateToken(user, roles, permissions);

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
       

        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return Result.Failure(UserError.InvalidCredentials);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserError.UserDisabled);

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
            await userManager.AddToRoleAsync(user, "Member");
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
        public async Task<Result> SendResetPasswordAsync(ForgetPasswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) return Result.Success();
            if (user.EmailConfirmed)
                return Result.Failure(UserError.InvalidCredentials);
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            logger.LogInformation("Reset password code for {email} is {code}", user.Email, code);

            await SendResetPasswordConfirmationAsync(user, code);
            return Result.Success();
        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

            var result = await userManager.ResetPasswordAsync(user!, code, request.NewPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
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
            //TODO: Use Hangfire to send email in background
            //BackgroundJob.Enqueue(() => emailService.SendEmailAsync(user.Email!, emailBody));

            await Task.CompletedTask;
        }
        private async Task SendResetPasswordConfirmationAsync(AppUser user, string code)
        {
            var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
            {
                {"{{name}}", $"{user.FirstName}" },
                {"{{action_url}}", $"{origin}/auth/forget-password?userid={user.Email}&code={code}" },

            });
            await emailService.SendEmailAsync(user.Email!, emailBody);
            //TODO: Use Hangfire to send email in background
            //BackgroundJob.Enqueue(() => emailService.SendEmailAsync(user.Email!, emailBody));

            await Task.CompletedTask;
        }

        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(AppUser user, CancellationToken cancellationToken)
        {
            var roles = await userManager.GetRolesAsync(user);
            var permissions =
                await (from r in context.Roles
                       join c in context.RoleClaims
                       on r.Id equals c.RoleId
                       where roles.Contains(r.Name!)
                       select c.ClaimValue)
                    .Distinct().ToListAsync(cancellationToken);
            return (roles, permissions);
        }
    }
}
