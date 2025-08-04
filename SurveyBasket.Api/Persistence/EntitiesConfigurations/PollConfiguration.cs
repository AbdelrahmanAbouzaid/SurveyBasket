using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace SurveyBasket.Api.Persistence.EntitiesConfigurations
{
    public class PollConfiguration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.HasIndex(p => p.Title).IsUnique();

            builder.Property(p => p.Title).HasMaxLength(100);
            builder.Property(p => p.Summary).HasMaxLength(1500);

            builder.HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
