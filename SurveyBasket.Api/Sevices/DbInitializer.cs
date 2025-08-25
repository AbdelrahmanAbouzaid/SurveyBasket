
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Sevices
{
    public class DbInitializer(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager) : IDbInitializer
    {
        public async Task InitializeAsync()
        {
            if (context.Database.GetPendingMigrations().Any())
                await context.Database.MigrateAsync();

            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!context.Users.Any())
            {
                var user = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Admin@123");
                await userManager.AddToRoleAsync(user, "Admin");

            }
        }
    }
}
