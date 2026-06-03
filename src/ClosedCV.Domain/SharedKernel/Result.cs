using System.Diagnostics.CodeAnalysis;

namespace ClosedCV.Domain.SharedKernel;

public record Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; init; } = Error.None;

    protected Result(bool isSuccess, Error error)
    {
        switch (isSuccess)
        {
            case true when error == Error.None:
                IsSuccess = true;
                Error = Error.None;
                break;

            case false when error != Error.None:
                IsSuccess = false;
                Error = error;
                break;

            default:
                throw new ArgumentException("Invalid result state");
        }
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);


    public static implicit operator Result(Error error) => Failure(error);
}

public record Result<T> : Result
{
    private readonly T? _value;

    [NotNull]
    public T Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access value of failed result");

    private Result(bool isSuccess, Error error, T value) : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(true, Error.None, value);
    public static new Result<T> Failure(Error error) => new(false, error, default!);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
