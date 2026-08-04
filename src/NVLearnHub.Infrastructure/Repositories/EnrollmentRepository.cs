using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Enrollment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(LearnHubDbContext context) : base(context) { }

        public async Task<IEnumerable<Enrollment>> GetByUserAsync(int userId)
            => await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Category)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .Where(e => e.UserId == userId)
                .ToListAsync();

        public async Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId)
            => await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        public async Task<bool> IsEnrolledAsync(int userId, int courseId)
            => await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }
}