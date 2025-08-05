using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace SurveyBasket.Api.Entities
{
    public class AuditableEntity
    {
        public string CreatedById { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedById { get; set; } 
        public DateTime? UpdatedAt { get; set; } 

        public AppUser CreatedBy { get; set; } = default!;
        public AppUser? UpdatedBy { get; set; }
    }
}
