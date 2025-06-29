using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Auth;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Sevices
{
    public class AuthServices(UserManager<AppUser> userManager, IJwtProvider jwtProvider) : IAuthServices
    {
        private readonly int _tokenExpiryDays = 15;

        public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = jwtProvider.ValidateToken(token);
            if (userId is null) return null;

            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return null;

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null ) return null;

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

            return new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                newToken,
                exprieIn,
                newRefreshToken,
                refreshTokenExpiry
                );
        }

        public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return null;

            var flag = await userManager.CheckPasswordAsync(user, password);
            if (!flag) return null;

            var (token, exprieIn) = jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_tokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiry
            });

            await userManager.UpdateAsync(user);

            return new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Token: token,
                exprieIn,
                refreshToken,
                refreshTokenExpiry
                );
        }


        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
