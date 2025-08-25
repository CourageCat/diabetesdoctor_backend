namespace UserService.Persistence.Configurations;

public class UserInfoConfiguration
    : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.OwnsOne(
           p => p.Avatar, condition =>
           {
               condition.Property(c => c.PublicId);

               condition.Property(c => c.Url)
                   .IsRequired();
           });

        builder.OwnsOne(
            p => p.FullName, condition =>
            {
                condition.Property(c => c.FirstName)
                    .IsRequired();

                condition.Property(c => c.MiddleName);

                condition.Property(c => c.LastName)
                    .IsRequired();
            });
        builder
            .HasOne(u => u.PatientProfile)
            .WithOne(p => p.User)
            .HasForeignKey<PatientProfile>(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(u => u.AdminProfile)
            .WithOne(p => p.User)
            .HasForeignKey<AdminProfile>(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(u => u.ModeratorProfile)
            .WithOne(p => p.User)
            .HasForeignKey<ModeratorProfile>(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(u => u.HospitalStaff)
            .WithOne(p => p.User)
            .HasForeignKey<HospitalStaff>(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(u => u.DoctorProfile)
            .WithOne(p => p.User)
            .HasForeignKey<DoctorProfile>(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(u => u.Wallet)
            .WithOne(p => p.User)
            .HasForeignKey<Wallet>(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
