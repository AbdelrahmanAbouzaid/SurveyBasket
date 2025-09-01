namespace SurveyBasket.Api.Contracts.Role
{
    public record RoleDetailsResponse(
        string Id,
        string Name,
        bool IsDeleted,
        IEnumerable<string> Permissions
        );
}
