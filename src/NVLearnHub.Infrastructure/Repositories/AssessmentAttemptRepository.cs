using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Assessment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class AssessmentAttemptRepository : Repository<AssessmentAttempt>, IAssessmentAttemptRepository
    {
        public AssessmentAttemptRepository(LearnHubDbContext context) : base(context) { }

        public async Task<IEnumerable<AssessmentAttempt>> GetByUserAsync(int userId)
            => await _context.AssessmentAttempts
                .Where(a => a.UserId == userId)
                .ToListAsync();

        public async Task<AssessmentAttempt?> GetWithAnswersAsync(int attemptId)
            => await _context.AssessmentAttempts
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

        public async Task<IEnumerable<AssessmentAttempt>> GetByAssessmentAsync(int assessmentId)
            => await _context.AssessmentAttempts
                .Where(a => a.AssessmentId == assessmentId)
                .ToListAsync();
    }
}