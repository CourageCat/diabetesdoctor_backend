namespace UserService.Persistence.Configurations;

public class UserPackageConfiguration
    : IEntityTypeConfiguration<UserPackage>
{
    public void Configure(EntityTypeBuilder<UserPackage> builder)
    {
        builder.HasKey(h => h.Id);
        builder
            .HasOne(up => up.PaymentHistory)
            .WithOne(ph => ph.UserPackage)
            .HasForeignKey<PaymentHistory>(ph => ph.UserPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}