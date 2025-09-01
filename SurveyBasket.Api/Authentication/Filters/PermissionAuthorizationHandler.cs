


namespace SurveyBasket.Api.Authentication.Filters
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //if (context.User.Identity is not { IsAuthenticated: true}
            //|| !context.User.Claims.Any(x => x.Value == requirement.Permission))
            //    return;

            var auth = context.User.Identity;
            if (auth == null || !auth.IsAuthenticated)
                return;
            if (requirement == null) return;
            var permission = context.User.Claims.Any(x => x.Value == requirement.Permission );
            if (!permission)
                return;
           

            context.Succeed(requirement);
        }
    }
}
