using CleanStart.Infrastructure.Service.Helpers;
using CleanStart.Service.Abstractions.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace CleanStart.Infrastructure.Service;

public static class DependencyInjection
{
  public static IServiceCollection AddMinimalHttpClientLogging(this IServiceCollection services)
  {
    services.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, MinimalHttpClientLogFilter>());
    return services;
  }

  public static IServiceCollection AddRequestContextProvider(this IServiceCollection services)
  {
    services.AddScoped<IRequestContextProvider, RequestContextProvider>();

    return services;
  }
}
