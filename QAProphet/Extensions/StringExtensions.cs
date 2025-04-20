namespace QAProphet.Extensions;

internal static class StringExtensions
{
    public static bool IsGuid(this string id) => Guid.TryParse(id, out _);
}