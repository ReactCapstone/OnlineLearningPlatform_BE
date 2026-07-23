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
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Lesson> Lessons => Set<Lesson>();

        // Enrollment
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Certificate> Certificates => Set<Certificate>();

        // Assessment
        public DbSet<Assessment> Assessments => Set<Assessment>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
        public DbSet<AssessmentAttempt> AssessmentAttempts => Set<AssessmentAttempt>();
        public DbSet<AssessmentAnswer> AssessmentAnswers => Set<AssessmentAnswer>();

        // Admin
        //public DbSet<Advertisement> Advertisements => Set<Advertisement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "system" },
                new Role { Id = 2, Name = "Student", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "system" }
            );
            modelBuilder.Entity<Course>()
                        .Property(c => c.Price)
                        .HasPrecision(18, 2);

            // Assessment seed
            //modelBuilder.Entity<Assessment>().HasData(
            //    new Assessment
            //    {
            //        Id = 1,
            //        CourseId = 1,
            //        Title = "C# Basics Quiz",
            //        TimeLimitMinutes = 30,
            //        PassPercentage = 70,
            //        CreatedAt = new DateTime(2024, 1, 1),
            //        CreatedBy = "system"
            //    }
            //);

            // Questions seed
            //modelBuilder.Entity<Question>().HasData(
            //    new Question { Id = 1, AssessmentId = 1, QuestionText = "What is a class in C#?", OrderIndex = 1, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new Question { Id = 2, AssessmentId = 1, QuestionText = "What keyword is used to inherit?", OrderIndex = 2, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new Question { Id = 3, AssessmentId = 1, QuestionText = "What is encapsulation?", OrderIndex = 3, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" }
            //);

            // Options seed
            //modelBuilder.Entity<QuestionOption>().HasData(
            //    // Question 1
            //    new QuestionOption { Id = 1, QuestionId = 1, OptionText = "A blueprint for objects", IsCorrect = true, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 2, QuestionId = 1, OptionText = "A variable", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 3, QuestionId = 1, OptionText = "A method", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 4, QuestionId = 1, OptionText = "A loop", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },

            //    // Question 2
            //    new QuestionOption { Id = 5, QuestionId = 2, OptionText = "extends", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 6, QuestionId = 2, OptionText = "inherits", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 7, QuestionId = 2, OptionText = ":", IsCorrect = true, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 8, QuestionId = 2, OptionText = "base", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },

            //    // Question 3
            //    new QuestionOption { Id = 9, QuestionId = 3, OptionText = "Hiding internal details", IsCorrect = true, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 10, QuestionId = 3, OptionText = "Having multiple forms", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 11, QuestionId = 3, OptionText = "Inheriting from a class", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" },
            //    new QuestionOption { Id = 12, QuestionId = 3, OptionText = "Breaking code into methods", IsCorrect = false, CreatedAt = new DateTime(2024, 1, 1), CreatedBy = "system" }
            //);

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
