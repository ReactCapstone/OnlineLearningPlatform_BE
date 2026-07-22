using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Enrollment;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(LearnHubDbContext context) : base(context) { }

        public async Task<IEnumerable<Wishlist>> GetByUserAsync(int userId)
            => await _context.Wishlists
                .Include(w => w.Course)
                    .ThenInclude(c => c.Category)
                .Include(w => w.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .Where(w => w.UserId == userId)
                .ToListAsync();

        public async Task<Wishlist?> GetByUserAndCourseAsync(int userId, int courseId)
            => await _context.Wishlists
                .Include(w => w.Course)
                    .ThenInclude(c => c.Category)
                .Include(w => w.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.CourseId == courseId);

        public async Task<bool> ExistsAsync(int userId, int courseId)
            => await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.CourseId == courseId);
    }
}