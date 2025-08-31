using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Entities
{
    public class AppRole : IdentityRole
    {
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}
