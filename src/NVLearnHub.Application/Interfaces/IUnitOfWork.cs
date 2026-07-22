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

        Task<int> SaveChangesAsync();
    }
}