using CleanStart.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanStart.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services,
                                                 IConfiguration configuration,
                                                 string connectionStringSection)
    {
        services.AddDbContext<CleanStartDbContext>(x => x.UseNpgsql(configuration.GetConnectionString(connectionStringSection)));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        ///TODO: add repositories here

        return services;
    }
}
