namespace Todo.Application.Common.Models;

public enum ResultErrorType
{
    Validation = 0,
    NotFound = 1,
    Conflict = 2,
    Unexpected = 3
}

public record ResultError(string Code, string Message, ResultErrorType Type)
{
    public List<string>? Details { get; init; }

    public static ResultError Validation(string message, List<string>? details = null) => 
        new("Validation.Error", message, ResultErrorType.Validation) { Details = details };

    public static ResultError NotFound(string message) => 
        new("NotFound.Error", message, ResultErrorType.NotFound);

    public static ResultError Conflict(string message) => 
        new("Conflict.Error", message, ResultErrorType.Conflict);

    public static ResultError Unexpected(string message) => 
        new("Unexpected.Error", message, ResultErrorType.Unexpected);
}

public class Result
{
    protected Result(bool isSuccess, ResultError? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error == null)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ResultError? Error { get; }

    public static Result Success() => new(true, null);
    public static Result Failure(ResultError error) => new(false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    private Result(bool isSuccess, ResultError? error, T? value) : base(isSuccess, error)
    {
        _value = value;
    }

    public T? Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Value can only be accessed for successful results.");

    public static Result<T> Success(T value) => new(true, null, value);
    public new static Result<T> Failure(ResultError error) => new(false, error, default);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(ResultError error) => Failure(error);
}
