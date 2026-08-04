using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Enrollment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class LessonProgressRepository : Repository<LessonProgress>, ILessonProgressRepository
    {
        public LessonProgressRepository(LearnHubDbContext context) : base(context) { }

        public async Task<IEnumerable<LessonProgress>> GetByEnrollmentAsync(int enrollmentId)
            => await _context.LessonProgress
                .Include(lp => lp.Lesson)
                .Where(lp => lp.EnrollmentId == enrollmentId)
                .ToListAsync();

        public async Task<LessonProgress?> GetByEnrollmentAndLessonAsync(int enrollmentId, int lessonId)
            => await _context.LessonProgress
                .FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollmentId
                                        && lp.LessonId == lessonId);

        public async Task<int> GetCompletedCountAsync(int enrollmentId)
            => await _context.LessonProgress
                .CountAsync(lp => lp.EnrollmentId == enrollmentId && lp.IsCompleted);
    }
}