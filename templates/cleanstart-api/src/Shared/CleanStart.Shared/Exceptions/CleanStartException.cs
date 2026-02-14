namespace CleanStart.Shared.Exceptions;

public class CleanStartException : Exception
{
    public ExceptionCode Code { get; private set; }

    protected CleanStartException(string message, ExceptionCode code) : base(message)
    {
        Code = code;
    }

    public static CleanStartException Create(string message, ExceptionCode code = ExceptionCode.InternalServerError)
    {
        return new CleanStartException(message, code);
    }

    public static CleanStartException BadRequest(string message)
    {
        return new CleanStartException(message, ExceptionCode.BadRequest);
    }
}
