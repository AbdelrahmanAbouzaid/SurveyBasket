using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Extentions
{
    public static class ErrorExtentions
    {
        public static Result FirstErrorResult(this IEnumerable<IdentityError> errors, int statusCode = StatusCodes.Status400BadRequest)
        {
            var error = errors.First();
            return Result.Failure(new Error(error.Code, error.Description, statusCode));
        }
        public static Result<Type> FirstErrorResult<Type>(this IEnumerable<IdentityError> errors, int statusCode = StatusCodes.Status400BadRequest)
        {
            var error = errors.First();
            return Result.Failure<Type>(new Error(error.Code, error.Description, statusCode));
        }
    }
}
