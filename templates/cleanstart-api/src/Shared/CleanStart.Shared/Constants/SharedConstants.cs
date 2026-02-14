using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace CleanStart.Shared.Constants;

public static class SharedConstants
{
    public static class HttpHeaders
    {
        public const string XCorrelationId = "X-Correlation-Id";
        public const string XSystemKey = "x-system-key";
        public const string XClientKey = "x-client-key";
    }

    public static class DateFormats
    {
        public const string YYYYMMDD = "yyyy-MM-dd";
        public const string YYYYMMDDTHHmmss = "yyyy-MM-ddTHH:mm:ss";

        public const string YYYYMMDDTHHmmssfffZ = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public const string IsoFormat = YYYYMMDDTHHmmssfffZ;
    }

    public static class Json
    {
        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, //the quotation mark is encoded as \" rather than \u0022.
        };

        public static readonly JsonSerializerOptions UnicodeRangesAll = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        public static readonly JsonSerializerOptions IgnoreCycles = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public static readonly JsonSerializerOptions PropertyNamingCamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public static class ApplicationNames
    {
        public const string CleanStartApi = "CleanStart.Api";
    }
}
