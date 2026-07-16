using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVLearnHub.Domain.Entities.Assessment;

namespace NVLearnHub.Infrastructure.Configurations
{
    public class AssessmentAttemptConfiguration : IEntityTypeConfiguration<AssessmentAttempt>
    {
        public void Configure(EntityTypeBuilder<AssessmentAttempt> builder)
        {
            builder.HasOne(a => a.User)
                   .WithMany()
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // ← prevents multiple cascade paths
        }
    }

    public class AssessmentAnswerConfiguration : IEntityTypeConfiguration<AssessmentAnswer>
    {
        public void Configure(EntityTypeBuilder<AssessmentAnswer> builder)
        {
            builder.HasOne(a => a.Question)
                   .WithMany()
                   .HasForeignKey(a => a.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.SelectedOption)
                   .WithMany()
                   .HasForeignKey(a => a.SelectedOptionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}