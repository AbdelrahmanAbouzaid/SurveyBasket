namespace SurveyBasket.Api.Abstractions
{
    public class Result
    {
        public Result(bool isSuccess, Error error)
        {
            if ((IsSuccess && error != Error.None))
                throw new ArgumentException("Invalid error state for the result.");
            IsSuccess = isSuccess;
            Error = error ?? throw new ArgumentNullException("Invalid error state for the result.");
        }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public Error Error { get; } = default!;

        public static Result Success() => new Result(true, Error.None);
        public static Result Failure(Error error) => new Result(false, error);

        public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(true, Error.None, value);
        public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(false, error, default);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue? _value;
        public Result(bool isSuccess, Error error, TValue? value)
            : base(isSuccess, error)
        {
            _value = value;
        }
        public TValue? Value => IsSuccess ? _value
            : throw new InvalidOperationException("Cannot access Value when the result is a failure.");

    }
}

