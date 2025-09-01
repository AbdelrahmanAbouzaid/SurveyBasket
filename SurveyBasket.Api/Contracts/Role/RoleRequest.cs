namespace SurveyBasket.Api.Contracts.Role
{
    public record RoleRequest(
        string Name,
        IList<string> Permissions
        );
   
}
