namespace SurveyBasket.Api.Abstractions
{
    public static class RateLimitPolicies
    {
        public const string Concurrency = "concurrency";
        public const string SlidingWindow = "slidingWindow";
        public const string TokenBucket = "tokenBucket";
        public const string FixedWindow = "fixedWindow";

        public const string UserLimit = "userLimit";
        public const string IpLimit = "ipLimit";

    }
}
