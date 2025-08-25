using System.Text.Json;

namespace UserService.Persistence.Configurations;

public class PatientProfileConfiguration
    : IEntityTypeConfiguration<PatientProfile>
{
    public void Configure(EntityTypeBuilder<PatientProfile> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.HasIndex(p => p.UserId);

        builder.OwnsOne(
           p => p.DiagnosisInfo, condition =>
           {
               condition.Property(c => c.Year);

               condition.Property(c => c.DiagnosisRecency)
                   .IsRequired();
           });

        builder.OwnsOne(
            p => p.DiabetesCondition, condition =>
            {
                condition.Property(c => c.DiabetesType)
                    .IsRequired();

                condition.Property(c => c.Type2TreatmentMethod);

                condition.Property(c => c.ControlLevel);

                condition.Property(c => c.InsulinFrequency);

                condition.Property(c => c.HasComplications)
                    .IsRequired();

                condition.Property(c => c.OtherComplicationDescription)
                    .HasMaxLength(255);

                condition.Property(c => c.ExerciseFrequency);

                condition.Property(c => c.EatingHabit);

                condition.Property(c => c.UsesAlcoholOrTobacco);

                condition.Property(c => c.Complications)
                    .HasColumnType("jsonb")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                        v => JsonSerializer.Deserialize<List<ComplicationType>>(v, new JsonSerializerOptions()) ?? new(),
                        new ValueComparer<List<ComplicationType>>(
                            (c1, c2) => c1!.SequenceEqual(c2!),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()
                        )
                    );
            });

        builder
            .HasMany(p => p.CarePlanMeasurementTemplates)
            .WithOne(c => c.PatientProfile)
            .HasForeignKey(c => c.PatientProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Navigation(p => p.CarePlanMeasurementTemplates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
