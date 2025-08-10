using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Auth;
using SurveyBasket.Api.Errors;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Sevices
{
    public class AuthServices(UserManager<AppUser> userManager, IJwtProvider jwtProvider) : IAuthServices
    {
        private readonly int _tokenExpiryDays = 15;

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
        {
            var userId = jwtProvider.ValidateToken(refreshTokenRequest.Token);
            if (userId is null)
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials); 

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenRequest.RefreshToken && x.IsActive);
            if (userRefreshToken is null ) return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

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


        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
