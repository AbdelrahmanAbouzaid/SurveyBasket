namespace SurveyBasket.Api.Contracts.User
{
    public record UserProfileResponse(
        string FirstName,
        string LastName,
        string UserName,
        string Email
        );

}
