using NVLearnHub.Application.DTOs.Wishlist;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Domain.Entities.Enrollment;

namespace NVLearnHub.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _uow;

        public WishlistService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ─── Get Wishlist ─────────────────────────────────────────────────────

        public async Task<ApiResponse<List<WishlistCourseDto>>> GetWishlistAsync(int userId, string? search = null)
        {
            var wishlistItems = await _uow.Wishlists.GetByUserAsync(userId);

            var dtos = wishlistItems
                .Select(w => MapToDto(w))
                .ToList();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                dtos = dtos.Where(c =>
                    c.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Category.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return new ApiResponse<List<WishlistCourseDto>>(dtos, "Wishlist retrieved successfully.");
        }

        // ─── Add to Wishlist ──────────────────────────────────────────────────

        public async Task<ApiResponse<WishlistCourseDto>> AddToWishlistAsync(int userId, int courseId)
        {
            // Check course exists
            var course = await _uow.Courses.GetByIdAsync(courseId);
            if (course == null)
                return new ApiResponse<WishlistCourseDto>(false, "Course not found.", 404);

            // Check already in wishlist
            if (await _uow.Wishlists.ExistsAsync(userId, courseId))
                return new ApiResponse<WishlistCourseDto>(false, "Course is already in your wishlist.", 409);

            // Add to wishlist
            var wishlist = new Wishlist
            {
                UserId = userId,
                CourseId = courseId
            };

            await _uow.Wishlists.AddAsync(wishlist);
            await _uow.SaveChangesAsync();

            // Fetch with full course details for response
            var added = await _uow.Wishlists.GetByUserAndCourseAsync(userId, courseId);

            return new ApiResponse<WishlistCourseDto>(
                MapToDto(added!),
                "Course added to wishlist.");
        }

        // ─── Remove from Wishlist ─────────────────────────────────────────────

        public async Task<ApiResponse<bool>> RemoveFromWishlistAsync(int userId, int courseId)
        {
            var wishlist = await _uow.Wishlists.GetByUserAndCourseAsync(userId, courseId);
            if (wishlist == null)
                return new ApiResponse<bool>(false, "Course not found in your wishlist.", 404);

            _uow.Wishlists.Remove(wishlist);
            await _uow.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Course removed from wishlist.");
        }

        // ─── Mapper ───────────────────────────────────────────────────────────

        private static WishlistCourseDto MapToDto(Wishlist w)
        {
            var totalLessons = w.Course.Sections?
                .SelectMany(s => s.Lessons)
                .Count() ?? 0;

            return new WishlistCourseDto
            {
                Id = w.Course.Id,
                Title = w.Course.Title,
                Description = w.Course.Description,
                Thumbnail = w.Course.Thumbnail,
                Category = w.Course.Category?.Name ?? string.Empty,
                Level = w.Course.Level,
                Price = w.Course.Price,
                TotalLessons = totalLessons,
                TotalSections = w.Course.Sections?.Count ?? 0
            };
        }
    }
}