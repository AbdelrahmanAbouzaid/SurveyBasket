namespace SurveyBasket.Api.Contracts.Consts
{
    public static class Permissions
    {
        public static string Type { get; } = "Permissions";

        public const string ReadPolls = "polls.read";
        public const string CreatePolls = "polls.create";
        public const string UpdatePolls = "polls.update";
        public const string DeletePolls = "polls.delete";

        public const string ReadQuestions = "questions.read";
        public const string CreateQuestions = "questions.create";
        public const string UpdateQuestions = "questions.update";

        public const string ReadUsers = "users.read";
        public const string CreateUsers = "users.create";
        public const string UpdateUsers = "users.update";

        public const string ReadRoles = "roles.read";
        public const string CreateRoles = "roles.create";
        public const string UpdateRoles = "roles.update";

        public const string ReadResults = "results.read";

        public static IList<string?> GetAllPermissions() =>
            typeof(Permissions).GetFields()
            .Select(x => x.GetValue(x) as string).ToList();


    }
}