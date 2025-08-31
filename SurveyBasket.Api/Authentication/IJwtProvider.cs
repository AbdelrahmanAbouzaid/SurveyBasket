namespace SurveyBasket.Api.Authentication
{
    public interface IJwtProvider
    {
        (string token, int expiresIn) GenerateToken(AppUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        string? ValidateToken(string token);
    }
}
