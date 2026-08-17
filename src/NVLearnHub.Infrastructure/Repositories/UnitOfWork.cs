using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Enrollment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LearnHubDbContext _context;

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IPasswordResetTokenRepository PasswordResetTokens { get; }
        public IAssessmentRepository Assessments { get; }
        public IAssessmentAttemptRepository AssessmentAttempts { get; }
        public IAssessmentAnswerRepository AssessmentAnswers { get; }
        public IWishlistRepository Wishlists { get; }
        public ICourseRepository Courses { get; }
        public IEnrollmentRepository Enrollments { get; }
        public ILessonProgressRepository LessonProgress { get; }
        public ICertificateRepository Certificates { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(LearnHubDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            PasswordResetTokens = new PasswordResetTokenRepository(_context);
            Assessments = new AssessmentRepository(_context);
            AssessmentAttempts = new AssessmentAttemptRepository(_context);
            AssessmentAnswers = new AssessmentAnswerRepository(_context);
            Wishlists = new WishlistRepository(_context);
            Courses = new CourseRepository(_context);
            Enrollments = new EnrollmentRepository(_context);
            LessonProgress = new LessonProgressRepository(_context);
            Certificates = new CertificateRepository(_context);
            Categories = new CategoryRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}