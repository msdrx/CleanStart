using CleanStart.Service.Abstractions.Helpers;
using CleanStart.Service.Abstractions.Integrations.Base;
using CleanStart.Shared.Constants;
using CleanStart.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;

namespace CleanStart.Infrastructure.Service.Intregrations.Base;

public abstract class GenericHttpClient : IGenericHttpClient
{
    protected readonly string _key;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILogger _logger;
    protected readonly IRequestContextProvider _contextProvider;

    public GenericHttpClient(string key, IHttpClientFactory httpClientFactory, ILogger logger, IRequestContextProvider contextProvider)
    {
        _httpClientFactory = httpClientFactory;
        _key = key;
        _logger = logger;
        _contextProvider = contextProvider;
    }

    public virtual async Task<T?> GetAsync<T>(string endpoint,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null)
    {
        endpoint = GetEndpoint(endpoint, queryParams);
        var http = GetHttpClient(headers);

        var httpResponse = cancellationToken is null ? await http.GetAsync(endpoint) : await http.GetAsync(endpoint, cancellationToken!.Value);
        var responseContent = await ValidateAndGetContent(httpResponse, endpoint);

        if (httpResponse!.StatusCode == HttpStatusCode.NoContent) return default;

        var result = JsonSerializer.Deserialize<T>(responseContent, jsonSerializerOptions);
        return result is null
            ? throw CleanStartException.Create($"GetAsync: after deserialize response is null endpoint={endpoint} content={responseContent}")
            : result;
    }

    public virtual async Task<T?> PostAsync<T, TPayload>(
        string endpoint,
        TPayload payload,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null)
    {
        endpoint = GetEndpoint(endpoint, queryParams);
        var http = GetHttpClient(headers);

        var httpResponse = cancellationToken is null ? await http.PostAsJsonAsync(endpoint, payload, options: jsonSerializerOptions)
                                                     : await http.PostAsJsonAsync(endpoint, payload, options: jsonSerializerOptions, cancellationToken!.Value);
        var responseContent = await ValidateAndGetContent(httpResponse, endpoint);

        if (httpResponse!.StatusCode == HttpStatusCode.NoContent) return default;

        var result = JsonSerializer.Deserialize<T>(responseContent, jsonSerializerOptions);
        return result is null
            ? throw CleanStartException.Create($"PostAsync: after deserialize response is null endpoint={endpoint} content={responseContent}")
            : result;
    }

    public virtual async Task<T?> PutAsync<T, TPayload>(
        string endpoint,
        TPayload payload,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null)
    {
        endpoint = GetEndpoint(endpoint, queryParams);
        var http = GetHttpClient(headers);

        var httpResponse = cancellationToken is null ? await http.PutAsJsonAsync(endpoint, payload, options: jsonSerializerOptions)
                                                     : await http.PutAsJsonAsync(endpoint, payload, options: jsonSerializerOptions, cancellationToken!.Value);
        var responseContent = await ValidateAndGetContent(httpResponse, endpoint);

        if (httpResponse!.StatusCode == HttpStatusCode.NoContent) return default;

        var result = JsonSerializer.Deserialize<T>(responseContent, jsonSerializerOptions);
        return result is null
            ? throw CleanStartException.Create($"PutAsync: after deserialize response is null endpoint={endpoint} content={responseContent}")
            : result;
    }

    public virtual async Task<T?> DeleteAsync<T>(
        string endpoint,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null)
    {
        endpoint = GetEndpoint(endpoint, queryParams);
        var http = GetHttpClient(headers);

        var httpResponse = cancellationToken is null ? await http.DeleteAsync(endpoint)
                                                     : await http.DeleteAsync(endpoint, cancellationToken!.Value);
        var responseContent = await ValidateAndGetContent(httpResponse, endpoint);

        if (httpResponse!.StatusCode == HttpStatusCode.NoContent) return default;

        var result = JsonSerializer.Deserialize<T>(responseContent, jsonSerializerOptions);
        return result is null
            ? throw CleanStartException.Create($"DeleteAsync: after deserialize response is null endpoint={endpoint} content={responseContent}")
            : result;
    }

    public virtual string GetQueryParams(IEnumerable<KeyValuePair<string, string>>? queryParams = null)
    {
        string result = string.Empty;
        if (queryParams?.Any() ?? false)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var kvp in queryParams)
            {
                query[kvp.Key] = kvp.Value;
            }
            result = query.ToString()!;
        }

        return result;
    }

    public virtual IEnumerable<KeyValuePair<string, string>>? GetDefaultHeaders()
    {
        return null;
    }

    private HttpClient GetHttpClient(IEnumerable<KeyValuePair<string, string>>? headers)
    {
        var http = _httpClientFactory?.CreateClient(_key) ?? throw ObjectNullException.Create($"{_key} http client is null");
        http.DefaultRequestHeaders.Add(SharedConstants.HttpHeaders.XCorrelationId, _contextProvider.Context.CorrelationId.ToString());

        var dhs = GetDefaultHeaders();
        if (dhs?.Any() ?? false)
        {
            foreach (var header in dhs)
            {
                http.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        if (headers?.Any() ?? false)
        {
            foreach (var header in headers)
            {
                http.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        return http;
    }

    private string GetEndpoint(string endpoint, IEnumerable<KeyValuePair<string, string>>? queryParams = null)
    {
        if (queryParams?.Any() ?? false)
        {
            endpoint = $"{endpoint}?{GetQueryParams(queryParams)}";
        }

        return endpoint;
    }

    private static async Task<string> ValidateAndGetContent(HttpResponseMessage? httpResponse, string endpoint, [CallerMemberName] string? caller = null)
    {
        var responseContent = httpResponse?.Content is null ? string.Empty : await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse?.IsSuccessStatusCode ?? true)
        {
            throw CleanStartException.Create($"{caller}: Http Status Code is invalide status={httpResponse?.StatusCode} endpoint={endpoint} content={responseContent}");
        }

        return responseContent;
    }
}

