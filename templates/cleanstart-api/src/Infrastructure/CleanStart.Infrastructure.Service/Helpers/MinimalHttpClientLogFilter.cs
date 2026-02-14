using System.Diagnostics;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Logging;

namespace CleanStart.Infrastructure.Service.Helpers;

internal class MinimalHttpClientLogFilter : IHttpMessageHandlerBuilderFilter
{
    private readonly ILoggerFactory _loggerFactory;

    public MinimalHttpClientLogFilter(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        return builder =>
        {
            next(builder);

            var loggerName = !string.IsNullOrEmpty(builder.Name) ? builder.Name : "Default";
            var innerLogger = _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.ClientHandler");
            var toRemove = builder.AdditionalHandlers.Where(h => (h is LoggingHttpMessageHandler) || h is LoggingScopeHttpMessageHandler).Select(h => h).ToList();
            foreach (var delegatingHandler in toRemove)
            {
                builder.AdditionalHandlers.Remove(delegatingHandler);
            }
            builder.AdditionalHandlers.Add(new MinimalLoggingHandler(innerLogger));
        };
    }

    internal class MinimalLoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public MinimalLoggingHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var stopwatch = ValueStopwatch.StartNew();
            try
            {
                _logger.LogInformation("http client start {RequestUri}", request.RequestUri);
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                var logLevel = LogLevel.Information;
                if (response.StatusCode >= System.Net.HttpStatusCode.InternalServerError) logLevel = LogLevel.Error;

                _logger.Log(logLevel, "http client end {time}: {RequestUri} - {StatusCode}",
                            stopwatch.GetElapsedTime().ToString(@"m\:ss\.fff"),
                            request.RequestUri,
                            response.StatusCode);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "http client end {time}: {RequestUri} - Error",
                                 stopwatch.GetElapsedTime().ToString(@"m\:ss\.fff"),
                                 request.RequestUri);
                throw;
            }
        }
    }
    internal struct ValueStopwatch
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        private long _startTimestamp;

        public bool IsActive => _startTimestamp != 0;

        private ValueStopwatch(long startTimestamp)
        {
            _startTimestamp = startTimestamp;
        }

        public static ValueStopwatch StartNew() => new ValueStopwatch(Stopwatch.GetTimestamp());

        public TimeSpan GetElapsedTime()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("An uninitialized, or 'default', ValueStopwatch cannot be used to get elapsed time.");
            }

            long end = Stopwatch.GetTimestamp();
            long timestampDelta = end - _startTimestamp;
            long ticks = (long)(TimestampToTicks * timestampDelta);
            return new TimeSpan(ticks);
        }
    }

}
