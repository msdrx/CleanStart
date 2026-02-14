using System.Text.Json;

namespace CleanStart.Service.Abstractions.Integrations.Base;

public interface IGenericHttpClient
{
    Task<T?> GetAsync<T>(string endpoint,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null);

    Task<T?> PostAsync<T, TPayload>(string endpoint,
        TPayload payload,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null);

    Task<T?> PutAsync<T, TPayload>(string endpoint,
        TPayload payload,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null);

    Task<T?> DeleteAsync<T>(string endpoint,
        IEnumerable<KeyValuePair<string, string>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken? cancellationToken = null);

    string GetQueryParams(IEnumerable<KeyValuePair<string, string>>? queryParams = null);
}