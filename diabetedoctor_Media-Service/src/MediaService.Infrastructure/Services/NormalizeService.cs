using MediaService.Contract.Infrastructure.Services;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaService.Infrastructure.Services;

public class NormalizeService : INormalizeService
{
    public string GetNormalizeString(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        string result = Regex.Replace(content.Trim(), @"\s+", " ");

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
}
