using NVLearnHub.Domain.Entities.Enrollment;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface ICertificateRepository : IRepository<Certificate>
    {
        Task<IEnumerable<Certificate>> GetByUserAsync(int userId);
        Task<Certificate?> GetByUserAndCourseAsync(int userId, int courseId);
        Task<bool> ExistsAsync(int userId, int courseId);
    }
}