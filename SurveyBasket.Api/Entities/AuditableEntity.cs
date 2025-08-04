namespace SurveyBasket.Api.Entities
{
    public class AuditableEntity
    {
        public string CreatedById { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string UpdatedById { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }

        public AppUser CreatedBy { get; set; } = default!;
        public AppUser? UpdatedBy { get; set; }
    }
}
