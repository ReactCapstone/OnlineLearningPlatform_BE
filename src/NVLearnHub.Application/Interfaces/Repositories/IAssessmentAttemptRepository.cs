using NVLearnHub.Domain.Entities.Assessment;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface IAssessmentAttemptRepository : IRepository<AssessmentAttempt>
    {
        Task<IEnumerable<AssessmentAttempt>> GetByUserAsync(int userId);
        Task<AssessmentAttempt?> GetWithAnswersAsync(int attemptId);
        Task<IEnumerable<AssessmentAttempt>> GetByAssessmentAsync(int assessmentId);
    }
}
