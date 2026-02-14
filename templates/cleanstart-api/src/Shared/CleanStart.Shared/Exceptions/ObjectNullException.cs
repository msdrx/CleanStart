namespace CleanStart.Shared.Exceptions;

public class ObjectNullException : CleanStartException
{
    protected ObjectNullException(string message, ExceptionCode code) : base(message, code)
    {
    }
}
