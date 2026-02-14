using System.Net;
using System.Text.Json.Serialization;

namespace CleanStart.Shared.Api.Dtos;

public interface IApiResponse
{
    HttpStatusCode Status { get; }
    public IEnumerable<KeyValuePair<string, string?>>? Messages { get; }

    [JsonIgnore]
    bool IsSuccessStatusCode { get; }
}
public interface IApiResponse<out T> : IApiResponse
{
    T? Result { get; }
}

public class ApiResponse : IApiResponse
{
    public HttpStatusCode Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<string, string?>>? Messages { get; set; }

    [JsonIgnore]
    public bool IsSuccessStatusCode { get { return Status >= HttpStatusCode.OK && Status < HttpStatusCode.Ambiguous; } }

    [JsonConstructor]
    public ApiResponse()
    {
    }
    protected ApiResponse(HttpStatusCode status, IEnumerable<KeyValuePair<string, string?>>? messages)
    {
        Status = status;
        Messages = messages;
    }

    public static IApiResponse StatusCode(HttpStatusCode statusCode, string? message = null, string messageKey = "StatusCodeMessageKey")
    {
        var errors = message == null ? null : new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>(messageKey, message)
        };

        return new ApiResponse(statusCode, errors);
    }

    public static IApiResponse Success(string? message = null)
    {
        return StatusCode(HttpStatusCode.OK, message, "OK");
    }

    public static IApiResponse Accepted(string? message = null)
    {
        return StatusCode(HttpStatusCode.Accepted, message, "Accepted");
    }

    public static IApiResponse BadRequest(string message)
    {
        return StatusCode(HttpStatusCode.BadRequest, message, "BadRequest");
    }
    public static IApiResponse BadRequest(IEnumerable<KeyValuePair<string, string?>>? messages)
    {
        return new ApiResponse(HttpStatusCode.BadRequest, messages);
    }

    public static IApiResponse Error(string message)
    {
        return StatusCode(HttpStatusCode.InternalServerError, message, "InternalServerError");
    }

    public static IApiResponse Unauthorized(string message = "მომხმარებელი არ არის ავტორიზებული")
    {
        return StatusCode(HttpStatusCode.Unauthorized, message, "Unauthorized");
    }

    public static IApiResponse Forbidden(string message = "მომხმარებელს არ აქვს უფლება")
    {
        return StatusCode(HttpStatusCode.Forbidden, message, "Forbidden");
    }
}

public class ApiResponse<T> : ApiResponse, IApiResponse<T>
{
    public T? Result { get; set; }

    [JsonConstructor]
    public ApiResponse()
    {
    }
    protected ApiResponse(T? result, HttpStatusCode status, IEnumerable<KeyValuePair<string, string?>>? messages) : base(status, messages)
    {
        Result = result;
    }

    public static IApiResponse<T> StatusCode(HttpStatusCode statusCode, T? result, string? message = null, string messagekey = "messageKey")
    {
        return new ApiResponse<T>(result, statusCode,
               message == null ? null : new List<KeyValuePair<string, string?>>()
               {
                   new KeyValuePair<string, string?>(messagekey, message)
               });
    }

    public static IApiResponse<T> Success(T? result, string? message = null)
    {
        return StatusCode(HttpStatusCode.OK, result, message, "OK");
    }

    public static IApiResponse<T> Accepted(T? result, string? message = null)
    {
        return StatusCode(HttpStatusCode.Accepted, result, message, "Accepted");
    }

    public static IApiResponse<T> Error(T? result, string? message = null)
    {
        return StatusCode(HttpStatusCode.InternalServerError, result, message, "InternalServerError");
    }
}
