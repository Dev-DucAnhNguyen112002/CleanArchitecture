namespace CleanArchitectureTest.Application.Common.Exceptions;

using CleanArchitectureTest.Contract.Models;

public abstract class ApplicationException : Exception
{
    public ErrorCode ErrorCode { get; }

    protected ApplicationException(ErrorCode errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected ApplicationException(ErrorCode errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

public class AppNotFoundException : ApplicationException
{
    public AppNotFoundException(string message) : base(ErrorCode.NotFound, message) { }
    public AppNotFoundException(string name, object key) : base(ErrorCode.NotFound, $"Entity \"{name}\" ({key}) was not found.") { }
    public AppNotFoundException(string entityName, string keyName, object keyValue)
    : base(ErrorCode.NotFound, $"Entity {entityName} with {keyName} {keyValue} not found.") { }
}

public class AppForbiddenException : ApplicationException
{
    public AppForbiddenException(string message) : base(ErrorCode.AccessDenied, message) { }
    public AppForbiddenException() : base(ErrorCode.AccessDenied, "Access is denied.") { }
}

public class AppBadRequestException : ApplicationException
{
    public AppBadRequestException(string message) : base(ErrorCode.FieldDataInvalid, message) { }
    public AppBadRequestException() : base(ErrorCode.FieldDataInvalid, "Bad request.") { }
}
