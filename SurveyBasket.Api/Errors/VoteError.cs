namespace SurveyBasket.Api.Errors
{
    public static class VoteError
    {
        public static Error InvalidOperation = new Error(
            "InvalidOperation",
            "Vote operation is invalid",
            StatusCodes.Status400BadRequest
            );

        public static Error DuplicateVote = new Error(
            "DuplicateVote",
            "Vote is already exists",
            StatusCodes.Status409Conflict
            );
        
    }
}
