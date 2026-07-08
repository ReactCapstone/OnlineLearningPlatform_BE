using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Domain.Common;
using NVLearnHub.Domain.Entities.Admin;
using NVLearnHub.Domain.Entities.Assessment;
using NVLearnHub.Domain.Entities.Catalog;
using NVLearnHub.Domain.Entities.Enrollment;
using NVLearnHub.Domain.Entities.Identity;


namespace NVLearnHub.Infrastructure.Data
{
    public class LearnHubDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public LearnHubDbContext(DbContextOptions<LearnHubDbContext> options,
                                  ICurrentUserService currentUser) : base(options)
            => _currentUser = currentUser;

        // Identity
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

        // Catalog
        //public DbSet<Category> Categories => Set<Category>();
        //public DbSet<Course> Courses => Set<Course>();
        //public DbSet<Section> Sections => Set<Section>();
        //public DbSet<Lesson> Lessons => Set<Lesson>();

        // Enrollment
        //public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        //public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();
        //public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        //public DbSet<Review> Reviews => Set<Review>();
        //public DbSet<Certificate> Certificates => Set<Certificate>();

        // Assessment
        //public DbSet<Assessment> Assessments => Set<Assessment>();
        //public DbSet<Question> Questions => Set<Question>();
        //public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
        //public DbSet<AssessmentAttempt> AssessmentAttempts => Set<AssessmentAttempt>();
        //public DbSet<AssessmentAnswer> AssessmentAnswers => Set<AssessmentAnswer>();

        // Admin
        //public DbSet<Advertisement> Advertisements => Set<Advertisement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "system" },
                new Role { Id = 2, Name = "Student", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "system" }
            );

            // Apply all IEntityTypeConfiguration classes in Infrastructure automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnHubDbContext).Assembly);

            // Apply BaseEntity audit field config to every entity inheriting it
            foreach (var entity in modelBuilder.Model.GetEntityTypes()
                         .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entity.ClrType)
                    .Property(nameof(BaseEntity.CreatedBy)).HasMaxLength(150);
                modelBuilder.Entity(entity.ClrType)
                    .Property(nameof(BaseEntity.UpdatedBy)).HasMaxLength(150);
            }

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var user = _currentUser.UserName ?? "system";

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = user;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = user;
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

}
