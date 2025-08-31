using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Contracts.Consts;



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

            var passwordHasher = new PasswordHasher<AppUser>();
            var password = passwordHasher.HashPassword(null!, DefaultUser.AdminPassword);
            builder.HasData( new AppUser
            {
                Id = DefaultUser.AdminId,
                UserName = DefaultUser.AdminEmail,
                NormalizedUserName = DefaultUser.AdminEmail.ToUpper(),
                Email = DefaultUser.AdminEmail,
                NormalizedEmail = DefaultUser.AdminEmail.ToUpper(),
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                SecurityStamp = DefaultUser.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUser.AdminConcurrencyStamp,
                PasswordHash = password
            });
        }
    }
}
