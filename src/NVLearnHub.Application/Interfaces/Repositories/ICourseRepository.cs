using NVLearnHub.Domain.Entities.Catalog;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetWithSectionsAndLessonsAsync(int courseId);
        Task<IEnumerable<Course>> SearchAsync(string? keyword, int? categoryId, string? level, string? sortBy);
        Task<IEnumerable<Course>> GetByInstructorAsync(int instructorId);
        Task<int> GetTotalLessonsCountAsync(int courseId);
    }
}