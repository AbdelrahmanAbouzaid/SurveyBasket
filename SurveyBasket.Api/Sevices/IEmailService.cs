namespace SurveyBasket.Api.Sevices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string htmlMessage);
    }
}