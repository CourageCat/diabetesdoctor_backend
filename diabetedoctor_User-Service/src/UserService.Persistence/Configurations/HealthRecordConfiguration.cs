using System.Text.Json;
using UserService.Domain.ValueObjects;
using UserService.Persistence.DependencyInjection.Extensions;

namespace UserService.Persistence.Configurations;

public class HealthRecordConfiguration
    : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.RecordValue)
             .HasColumnType("jsonb")
              .HasConversion(
                v => JsonSerializer.Serialize(v, v.GetType(), JsonSettings.Polymorphic),
                v => JsonSerializer.Deserialize<HealthRecordValue>(v, JsonSettings.Polymorphic)!
            );

        builder
            .HasOne(u => u.CarePlanMeasurementInstance)
            .WithOne(p => p.HealthRecord)
            .HasForeignKey<CarePlanMeasurementInstance>(p => p.HealthRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
