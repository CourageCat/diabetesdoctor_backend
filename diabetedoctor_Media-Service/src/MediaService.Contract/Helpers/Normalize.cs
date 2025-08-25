using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaService.Contract.Helpers;

public static partial class Normalize
{
    public static string GetNormalizeString(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        string result = MyRegex().Replace(content.Trim(), " ");

        result = RemoveDiacritics(result);

        result = result.ToLowerInvariant();

        return result;
    }
    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                builder.Append(ch);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegex();
}