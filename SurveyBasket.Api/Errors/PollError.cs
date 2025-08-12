namespace SurveyBasket.Api.Errors
{
    public static class PollError
    {
        public static Error NotFound(int id) =>
            new Error(
                "PollNotFound",
                $"Poll with ID {id} was not found."
            );

        public static readonly Error InvalidPollData = new Error(
                "InvalidPollData",
                "The provided poll data is invalid."
            );
        public static Error DuplicateTitle(string title) => 
            new Error(
                "DuplicatePollTitle",
                $"A poll with the title '{title}' already exists."
            );
    }
}
