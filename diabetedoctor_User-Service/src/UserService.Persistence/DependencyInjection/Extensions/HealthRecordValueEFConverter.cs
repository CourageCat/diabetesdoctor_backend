using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserService.Domain.ValueObjects;

namespace UserService.Persistence.DependencyInjection.Extensions;

public class HealthRecordValueEFConverter : ValueConverter<HealthRecordValue, string>
{
    public HealthRecordValueEFConverter()
        : base(
            v => JsonSerializer.Serialize(v, JsonSettings.Polymorphic),
            v => JsonSerializer.Deserialize<HealthRecordValue>(v, JsonSettings.Polymorphic)!
        )
    { }
}
