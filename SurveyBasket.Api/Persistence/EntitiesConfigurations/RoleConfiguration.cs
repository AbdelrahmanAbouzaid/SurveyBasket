using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Contracts.Consts;



namespace SurveyBasket.Api.Persistence.EntitiesConfigurations
{
    public class RoleConfiguration /*: IEntityTypeConfiguration<AppRole>*/
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {

            //builder.HasData([
            //    new AppRole
            //    {
            //        Id = DefaultRole.AdminRoleId,
            //        Name = DefaultRole.Admin,
            //        NormalizedName = DefaultRole.Admin.ToUpper(),
            //        ConcurrencyStamp = DefaultRole.AdminRoleConcurrencyStamp
            //    },
            //    new AppRole
            //    {
            //        Id = DefaultRole.MemberRoleId,
            //        Name = DefaultRole.Member,
            //        NormalizedName = DefaultRole.Member.ToUpper(),
            //        ConcurrencyStamp = DefaultRole.MemberRoleConcurrencyStamp,
            //        IsDefault = true
            //    }
            //    ]);
        }
    }
}
