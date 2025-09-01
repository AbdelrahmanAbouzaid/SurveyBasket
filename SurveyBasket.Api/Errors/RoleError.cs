using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.Api.Errors
{
    public static class RoleError
    {
        public static readonly Error RoleNotFound = new Error(
             "Role.NotFound",
             "The specified role was not found.",
             StatusCodes.Status404NotFound
            );

        public static readonly Error DuplicatedRole = new Error(
             "Role.Duplicated",
             "The specified role name already exists.",
             StatusCodes.Status409Conflict
            );

        public static readonly Error InvalidPermissions = new Error(
             "Role.InvalidPermissions",
             "One or more specified permissions are invalid.",
             StatusCodes.Status400BadRequest
            );
    }
}
