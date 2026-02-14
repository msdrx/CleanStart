using System.Net;
using CleanStart.Shared.Exceptions;

namespace CleanStart.Shared.Api.Extensions;

public static class ExceptionCodeExtensions
{
    public static HttpStatusCode ToHttpStatusCode(this ExceptionCode code)
    {
        switch (code)
        {
            case ExceptionCode.InternalServerError:
                return HttpStatusCode.InternalServerError;
            case ExceptionCode.BadRequest:
                return HttpStatusCode.BadRequest;
            default:
                return HttpStatusCode.InternalServerError;
        }
    }
}
