using System.Net;
using CleanStart.Shared.Exceptions;

namespace CleanStart.Domain.Exceptions;

public class DomainException : CleanStartException
{
    protected DomainException(string message, ExceptionCode code) : base(message, code)
    {
    }
}
