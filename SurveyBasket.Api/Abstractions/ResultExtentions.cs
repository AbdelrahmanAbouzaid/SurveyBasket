using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Abstractions
{
    public static class ResultExtentions
    {
        public static ObjectResult ToProblem(this Result result, int statusCode)
        {

            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert a successful result to a problem.");

            var proplem = Results.Problem(statusCode: statusCode);
            var problemDetails = proplem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(proplem) as ProblemDetails;
            problemDetails!.Extensions = new Dictionary<string, object?>()
            {
                { "errorS", new []{ result.Error} }
            };
           
            return new ObjectResult(problemDetails);
        }
    }
}
