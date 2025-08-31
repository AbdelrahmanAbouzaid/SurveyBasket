namespace SurveyBasket.Api.Contracts.Consts
{
    public static class Regex
    {
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    }
}
