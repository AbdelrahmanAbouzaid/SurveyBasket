using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations
{
    public class RoleCliamConfiguration/* : IEntityTypeConfiguration<IdentityRoleClaim<string>>*/
    {
        //public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        //{
        //    var permissions = Permissions.GetAllPermissions;
        //    var adminClaims = new List<IdentityRoleClaim<string>>();

        //    for (int i = 0; i < permissions.Length; i++)
        //    {
        //        adminClaims.Add(new IdentityRoleClaim<string>
        //        {
        //            Id = i + 1,
        //            RoleId = DefaultRole.AdminRoleId,
        //            ClaimType = Permissions.Type,
        //            ClaimValue = permissions[i] ?? string.Empty
        //        });
        //    }
        //    builder.HasData(adminClaims);
        //}
    }
}
