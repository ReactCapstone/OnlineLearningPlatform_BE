using NVLearnHub.Domain.Entities.Assessment;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface IAssessmentRepository : IRepository<Assessment>
    {
        Task<Assessment?> GetWithQuestionsAndOptionsAsync(int assessmentId);
        Task<Assessment?> GetByCourseAsync(int courseId);
    }
}
