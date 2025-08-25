namespace AuthService.Api.Abstractions.Exceptions;

public abstract class ForbiddenException : DomainException
{
    protected ForbiddenException(string message, string? errorCode = null)
        : base("Forbidden", message, errorCode)
    {
    }
}