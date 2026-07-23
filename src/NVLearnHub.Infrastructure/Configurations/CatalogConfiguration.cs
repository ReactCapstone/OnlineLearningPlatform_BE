using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVLearnHub.Domain.Entities.Catalog;

namespace NVLearnHub.Infrastructure.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasOne(c => c.Instructor)
                   .WithMany()
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Category)
                   .WithMany(c => c.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasOne(s => s.Course)
                   .WithMany(c => c.Sections)
                   .HasForeignKey(s => s.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasOne(l => l.Section)
                   .WithMany(s => s.Lessons)
                   .HasForeignKey(l => l.SectionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}