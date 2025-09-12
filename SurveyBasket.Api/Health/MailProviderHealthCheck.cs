using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.Health
{
    public class MailProviderHealthCheck(IOptions<MailSettings> options) : IHealthCheck
    {
        private readonly MailSettings options = options.Value;
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(options.Host, options.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(options.Mail, options.Password);

                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(exception: ex));
            }
        }
    }
}
