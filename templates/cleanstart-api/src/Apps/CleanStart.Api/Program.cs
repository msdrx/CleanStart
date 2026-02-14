using CleanStart.Shared.Api.Extensions;
using CleanStart.Shared.Api.Middlewares;
using CleanStart.Shared.Constants;
using NLog;
using NLog.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CleanStart.Infrastructure.Service;
using CleanStart.Application;

var logger = LogManager.Setup().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureServices(builder);

    var app = builder.Build();

    ConfigureMiddleware(app);
    ConfigureEndpoints(app);

    await app.RunAsync();

}
catch (Exception ex)
{
    logger.Error(ex, "{api} startup error", SharedConstants.ApplicationNames.CleanStartApi);
    throw;
}
finally
{
    LogManager.Shutdown();
}



static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers().AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }).ConfigureBadRequest();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(x =>
    {
        string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        x.SwaggerDoc("1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = SharedConstants.ApplicationNames.CleanStartApi, Version = "1.0" });
        x.IncludeXmlComments(xmlPath);
        x.UseInlineDefinitionsForEnums();
    });


    builder.Logging.ClearProviders();
    builder.Logging.AddNLog();

    //TODO: add validations


    builder.Services.AddMinimalHttpClientLogging();
    builder.Services.AddRequestContextProvider();

    builder.Services.AddApplicationServices();
}


static void ConfigureMiddleware(IApplicationBuilder app)
{
    app.UseError(SharedConstants.ApplicationNames.CleanStartApi);
    app.UseRequestContext();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"../swagger/1.0/swagger.json", $"{SharedConstants.ApplicationNames.CleanStartApi} {1.0}");
    });

    app.UseRouting();
    //TODO: use HealthChecks

    app.UseHttpsRedirection();

    app.UseAuthorization();
}

static void ConfigureEndpoints(IEndpointRouteBuilder app)
{
    app.MapControllers();
}