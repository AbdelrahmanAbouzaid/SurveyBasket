
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Contracts.Consts;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations
{
    public class UserRoleConfiguration /*: IEntityTypeConfiguration<IdentityUserRole<string>>*/
    {
        //public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        //{
        //    builder.HasData(
        //        new IdentityUserRole<string>
        //        {
        //            UserId = DefaultUser.AdminId,
        //            RoleId = DefaultRole.AdminRoleId,
        //        }
        //    );
        //}

    }
}
