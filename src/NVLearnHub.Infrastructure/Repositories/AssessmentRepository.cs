using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Assessment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class AssessmentRepository : Repository<Assessment>, IAssessmentRepository
    {
        public AssessmentRepository(LearnHubDbContext context) : base(context) { }

        public async Task<Assessment?> GetWithQuestionsAndOptionsAsync(int assessmentId)
            => await _context.Assessments
                .Include(a => a.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(a => a.Id == assessmentId);

        public async Task<Assessment?> GetByCourseAsync(int courseId)
            => await _context.Assessments
                .Include(a => a.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(a => a.CourseId == courseId);
    }
}