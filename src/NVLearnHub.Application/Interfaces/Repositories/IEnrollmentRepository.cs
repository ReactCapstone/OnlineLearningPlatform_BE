using NVLearnHub.Domain.Entities.Enrollment;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetByUserAsync(int userId);
        Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId);
        Task<bool> IsEnrolledAsync(int userId, int courseId);
    }
}