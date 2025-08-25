using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Api.Persistences.Configurations;

public class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(o => o.Id);

        builder.OwnsOne(
           u => u.Avatar, identityBuilder =>
           {
               identityBuilder.Property(i => i.PublicId)
                   .IsRequired();

               identityBuilder.Property(i => i.Url)
                   .IsRequired();
           });
    }
}

