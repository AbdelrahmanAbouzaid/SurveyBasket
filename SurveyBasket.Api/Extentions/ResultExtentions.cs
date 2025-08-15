using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Extentions
{
    public static class ResultExtentions
    {
        public static ObjectResult ToProblem(this Result result)
        {

            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert a successful result to a problem.");

            var proplem = Results.Problem(statusCode: result.Error.statusCode);
            var problemDetails = proplem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(proplem) as ProblemDetails;
            problemDetails!.Extensions = new Dictionary<string, object?>()
            {
                { "errors", new []{ result.Error.code, result.Error.description } }
            };
           
            return new ObjectResult(problemDetails);
        }
    }
}
