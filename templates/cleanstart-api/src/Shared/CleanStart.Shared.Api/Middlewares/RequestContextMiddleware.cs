using System.Diagnostics;
using CleanStart.Service.Abstractions.Helpers;
using CleanStart.Shared.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CleanStart.Shared.Api.Middlewares;

public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext, IRequestContextProvider requestContextProvider)
    {
        httpContext.Request.Headers.TryGetValue(SharedConstants.HttpHeaders.XCorrelationId, out StringValues val);
        var headerValue = val.ToString();

        Guid correlationId = string.IsNullOrWhiteSpace(headerValue) ? Guid.NewGuid() : Guid.Parse(headerValue);

        Trace.CorrelationManager.ActivityId = correlationId;

        requestContextProvider.SetContext(new RequestContext(correlationId, httpContext?.Connection?.RemoteIpAddress?.ToString()));

        return _next(httpContext!);
    }
}

public static class RequestContextMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestContextMiddleware>();
    }
}