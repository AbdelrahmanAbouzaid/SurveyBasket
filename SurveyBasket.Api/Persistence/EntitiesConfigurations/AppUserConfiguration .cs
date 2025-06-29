using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Entities;


namespace SurveyBasket.Api.Persistence.EntitiesConfigurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u => u.LastName).HasMaxLength(100);
            builder.OwnsMany(u => u.RefreshTokens)
                .ToTable("RefreshTokens")
                .WithOwner().HasForeignKey("UserId");
        }
    }
}
