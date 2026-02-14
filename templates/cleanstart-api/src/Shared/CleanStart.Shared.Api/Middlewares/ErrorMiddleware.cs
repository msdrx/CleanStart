using CleanStart.Shared.Api.Dtos;
using CleanStart.Shared.Api.Extensions;
using CleanStart.Shared.Constants;
using CleanStart.Shared.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace CleanStart.Shared.Api.Middlewares;

public class ErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _serviceName;
    public ErrorMiddleware(RequestDelegate next, string serviceName)
    {
        _next = next;
        _serviceName = serviceName;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<ErrorMiddleware> logger, IServiceProvider serviceProvider)
    {
        var watch = new Stopwatch();
        try
        {
            watch.Start();
            await _next(context);
        }
        catch (Exception ex)
        {
            try
            {
                IApiResponse response;
                if (ex is CleanStartException appException)
                {
                    response = ApiResponse.StatusCode(appException.Code.ToHttpStatusCode(), appException.Message);
                }
                else
                {
                    response = ApiResponse.Error(ex.Message);
                }

                if (response.Status == HttpStatusCode.BadRequest)
                {
                    logger.LogWarning(ex, "{serviceName}: validation error", _serviceName);
                }
                else
                {
                    logger.LogError(ex, "{serviceName}: global error", _serviceName);
                }

                var json = JsonSerializer.Serialize(response, SharedConstants.Json.UnicodeRangesAll);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)response.Status;

                if (response.Status == HttpStatusCode.InternalServerError)
                {
                    context.Response.Headers.Append("Application-Error", "error occured while proccessing offers");
                    context.Response.Headers.Append("Access-Control-Expose-Headers", "Application-Error");
                }

                await context.Response.WriteAsync(json).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError(e, "{serviceName}: error occured after response has already started requestPath={requestPath}, parentException={parentEception}", context.Request.Path.Value, ex, _serviceName);
            }
        }
        finally
        {
            watch.Stop();
            if (!context.Request.Path.Value?.Contains("swagger") ?? false)
            {
                var time = watch.Elapsed.ToString(@"m\:ss\.fff");
                logger.LogInformation("{serviceName} Request/Response time elapsed = {time}, {route}", _serviceName, time, context.Request.Path);
            }
        }
    }
}

public static class ErrorMidlewareExtensions
{
    public static void UseError(this IApplicationBuilder builder, string serviceName)
    {
        builder.UseMiddleware<ErrorMiddleware>(serviceName);
    }
}