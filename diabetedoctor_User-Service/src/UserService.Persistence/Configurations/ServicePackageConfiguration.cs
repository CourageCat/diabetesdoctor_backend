namespace UserService.Persistence.Configurations;

public class ServicePackageConfiguration
    : IEntityTypeConfiguration<ServicePackage>
{
    public void Configure(EntityTypeBuilder<ServicePackage> builder)
    {
        builder.HasKey(h => h.Id);
        builder
            .HasMany(p => p.UserPackage)
            .WithOne(c => c.ServicePackage)
            .HasForeignKey(c => c.ServicePackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}