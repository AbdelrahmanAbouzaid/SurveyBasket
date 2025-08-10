namespace SurveyBasket.Api.Errors
{
    public static class UserError
    {
        public static readonly Error InvalidCredentials = new Error("User.InvalidCredintial", "Invalid Email Or Password");
    }
}
