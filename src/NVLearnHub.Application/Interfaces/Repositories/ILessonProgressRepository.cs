using NVLearnHub.Domain.Entities.Enrollment;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface ILessonProgressRepository : IRepository<LessonProgress>
    {
        Task<IEnumerable<LessonProgress>> GetByEnrollmentAsync(int enrollmentId);
        Task<LessonProgress?> GetByEnrollmentAndLessonAsync(int enrollmentId, int lessonId);
        Task<int> GetCompletedCountAsync(int enrollmentId);
    }
}