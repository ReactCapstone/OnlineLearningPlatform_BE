using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Assessment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class AssessmentAnswerRepository : Repository<AssessmentAnswer>, IAssessmentAnswerRepository
    {
        public AssessmentAnswerRepository(LearnHubDbContext context) : base(context) { }
    }
}