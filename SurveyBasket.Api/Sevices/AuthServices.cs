using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Auth;

namespace SurveyBasket.Api.Sevices
{
    public class AuthServices(UserManager<AppUser> userManager, IJwtProvider jwtProvider) : IAuthServices
    {
        public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return null;

            var flag = await userManager.CheckPasswordAsync(user, password);
            if (!flag) return null;

            var (token, exprieIn) = jwtProvider.GenerateToken(user);

            return new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Token: token,
                exprieIn
                );
        }
    }
}
