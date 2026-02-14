using CleanStart.Shared.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CleanStart.Service.Abstractions.Helpers;

public interface IRequestContextProvider
{
    RequestContext Context { get; }
    void SetContext(RequestContext context);
}


public sealed class RequestContext
{
    public Guid CorrelationId { get; init; }
    public string? RemoteIpAddress { get; init; }


    public RequestContext(Guid correlationId, string? remoteIpAddress)
    {
        CorrelationId = correlationId;
        RemoteIpAddress = remoteIpAddress;
    }
}

public static class ServiceProviderExtensions
{
    /// <summary>
    /// takes <see cref="RequestContext"/> from current scope and sets into newly created scope
    /// </summary>
    /// <param name="provider"></param>
    /// <returns><see cref="IServiceScope"/></returns>
    public static IServiceScope CreateCustomScope(this IServiceProvider provider)
    {
        var oldRequestContextService = provider.GetRequiredService<IRequestContextProvider>();

        return provider.CreateCustomScope(oldRequestContextService.Context?.DeepCopy());
    }

    private static IServiceScope CreateCustomScope(this IServiceProvider provider, RequestContext? context)
    {
        if (context is null) throw new ArgumentException("CreateCustomScope: context is null");

        var scope = provider.CreateScope();

        //set RequestScopedContext for manually created scope
        var contextProvider = scope.ServiceProvider.GetRequiredService<IRequestContextProvider>();
        contextProvider.SetContext(context);

        return scope;
    }
}