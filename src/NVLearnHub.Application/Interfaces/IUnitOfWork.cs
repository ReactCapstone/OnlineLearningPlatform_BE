using NVLearnHub.Application.Interfaces.Repositories;

namespace NVLearnHub.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IPasswordResetTokenRepository PasswordResetTokens { get; }
        IAssessmentRepository Assessments { get; }
        IAssessmentAttemptRepository AssessmentAttempts { get; }
        IAssessmentAnswerRepository AssessmentAnswers { get; }
        IWishlistRepository Wishlists { get; }
        ICourseRepository Courses { get; }
        IEnrollmentRepository Enrollments { get; }
        ILessonProgressRepository LessonProgress { get; }
        ICertificateRepository Certificates { get; }
        ICategoryRepository Categories { get; }
        IEmailOtpRepository EmailOtps { get; }

        Task<int> SaveChangesAsync();
    }
}