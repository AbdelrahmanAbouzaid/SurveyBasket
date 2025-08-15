namespace SurveyBasket.Api.Abstractions
{
    public record Error(string code, string description, int? statusCode)
    {
        public static Error None => new Error(string.Empty, string.Empty, null);
    }
}