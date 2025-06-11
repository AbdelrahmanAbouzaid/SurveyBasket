using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace SurveyBasket.Api.Persistence.EntitiesConfigurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(p => p.LastName).HasMaxLength(100);
        }
    }
}
