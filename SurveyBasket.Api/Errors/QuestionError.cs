namespace SurveyBasket.Api.Errors
{
    public static class QuestionError
    {
        public static Error NotFound(int id) =>
            new Error(
                "QuestionNotFound",
                $"Question with ID {id} was not found.",
                StatusCodes.Status404NotFound
            );
        public static readonly Error InvalidQuestionData = new Error(
            "InvalidQuestionData",
            "The provided question data is invalid.",
            StatusCodes.Status400BadRequest
        );
        public static Error DuplicateTitle(string content) =>
            new Error(
                "DuplicateQuestionTitle",
                $"A question with the content '{content}' already exists.",
                StatusCodes.Status409Conflict
            );
    }
}
