namespace UserService.Persistence.Configurations;

public class HospitalProfileConfiguration
    : IEntityTypeConfiguration<HospitalProfile>
{
    public void Configure(EntityTypeBuilder<HospitalProfile> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.OwnsOne(
           p => p.Thumbnail, condition =>
           {
               condition.Property(c => c.PublicId);

               condition.Property(c => c.Url)
                   .IsRequired();
           });
    }
}
