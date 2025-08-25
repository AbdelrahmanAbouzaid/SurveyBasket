namespace SurveyBasket.Api.Errors
{
    public static class UserError
    {
        public static readonly Error InvalidCredentials = new Error(
            "User.InvalidCredintial",
            "Invalid Email Or Password",
            StatusCodes.Status401Unauthorized
            );
        public static readonly Error DuplicateEmail = new Error(
            "User.DuplicateEmail",
            "Email is already registered",
            StatusCodes.Status409Conflict
            );
        public static readonly Error DuplicateConfirmation = new Error(
            "User.DuplicateConfirmation",
            "Email is already confirmed",
            StatusCodes.Status409Conflict
            );
        public static readonly Error InvalidCode = new Error(
            "User.InvalidCode",
            "Invalid Code",
            StatusCodes.Status409Conflict
            );

    }
}
