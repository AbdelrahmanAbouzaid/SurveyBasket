using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Middlewares
{
    public class GlobalHandlingExceptionMiddleware(ILogger<GlobalHandlingExceptionMiddleware> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalHandlingExceptionMiddleware> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "something went wrong: {message}", exception.Message);
            var problemDetails = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = @"https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };
          
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
            return true;
        }
    }
}
