
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Persistence;
using System.Security.Claims;

namespace SurveyBasket.Api.Sevices
{
    public class DbInitializer(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager) : IDbInitializer
    {
        public async Task InitializeAsync()
        {
            if (context.Database.GetPendingMigrations().Any())
                await context.Database.MigrateAsync();

            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new AppRole() { Name = "Admin" });
                await roleManager.CreateAsync(new AppRole() { Name = "Member", IsDefault = true });
            }

            if (!context.RoleClaims.Any())
            {
                var adminRole = await roleManager.FindByNameAsync("Admin");
                var permissions = Permissions.GetAllPermissions();
                foreach (var cliam in permissions)
                {
                    await roleManager.AddClaimAsync(adminRole!, new Claim(Permissions.Type, cliam!));
                }
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
