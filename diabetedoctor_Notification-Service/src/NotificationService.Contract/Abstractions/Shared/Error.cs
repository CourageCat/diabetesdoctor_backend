namespace NotificationService.Contract.Abstractions.Shared;

public class Error : IEquatable<Error>
{
    public Error(string code, string message, ErrorType errorType)
    {
        Code = code;
        Message = message;
        ErrorType = errorType;
    }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.NotFound);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.", ErrorType.Failure);

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);
    public static Error NotFound(string code, string message) =>
    new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) =>
    new(code, message, ErrorType.Conflict);
    public static Error Forbidden(string code, string message) =>
    new(code, message, ErrorType.Forbidden);


    public string Code { get; }

    public string Message { get; }

    public ErrorType ErrorType { get; }

    public static implicit operator string(Error error) => error.Code;

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Error? a, Error? b) => !(a == b);

    public virtual bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message;
    }

    public override bool Equals(object? obj) => obj is Error error && Equals(error);

    public override int GetHashCode() => HashCode.Combine(Code, Message);

    public override string ToString() => Code;
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Forbidden = 4,
}