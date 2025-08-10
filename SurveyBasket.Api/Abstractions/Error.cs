namespace SurveyBasket.Api.Abstractions
{
    public record Error(string code, string description)
    {
        public static Error None => new Error(string.Empty, string.Empty);
    }
}