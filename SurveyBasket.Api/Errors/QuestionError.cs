namespace SurveyBasket.Api.Errors
{
    public static class QuestionError
    {
        public static Error NotFound(int id) =>
            new Error(
                "QuestionNotFound",
                $"Question with ID {id} was not found."
            );
        public static readonly Error InvalidQuestionData = new Error(
            "InvalidQuestionData",
            "The provided question data is invalid."
        );
        public static Error DuplicateTitle(string content) =>
            new Error(
                "DuplicateQuestionTitle",
                $"A question with the content '{content}' already exists."
            );
    }
}
