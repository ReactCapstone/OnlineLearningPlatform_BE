using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Enrollment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class CertificateRepository : Repository<Certificate>, ICertificateRepository
    {
        public CertificateRepository(LearnHubDbContext context) : base(context) { }

        public async Task<IEnumerable<Certificate>> GetByUserAsync(int userId)
            => await _context.Certificates
                .Include(c => c.Course)
                .Where(c => c.UserId == userId)
                .ToListAsync();

        public async Task<Certificate?> GetByUserAndCourseAsync(int userId, int courseId)
            => await _context.Certificates
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

        public async Task<bool> ExistsAsync(int userId, int courseId)
            => await _context.Certificates
                .AnyAsync(c => c.UserId == userId && c.CourseId == courseId);
    }
}