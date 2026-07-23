using NVLearnHub.Domain.Entities.Enrollment;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetByUserAsync(int userId);
        Task<Wishlist?> GetByUserAndCourseAsync(int userId, int courseId);
        Task<bool> ExistsAsync(int userId, int courseId);
    }
}