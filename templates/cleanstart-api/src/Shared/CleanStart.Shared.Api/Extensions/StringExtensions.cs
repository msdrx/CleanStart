namespace CleanStart.Shared.Api.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string first, string second)
    {
        return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
    }
}
