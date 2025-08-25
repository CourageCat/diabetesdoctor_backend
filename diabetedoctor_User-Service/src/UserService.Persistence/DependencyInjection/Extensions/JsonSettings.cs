using System.Text.Json;

namespace UserService.Persistence.DependencyInjection.Extensions;

public static class JsonSettings
{
    public static readonly JsonSerializerOptions Polymorphic = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new HealthRecordValueConverter(), new PackageFeatureValueConverter() }
    };
}
