using NVLearnHub.Application.DTOs.Wishlist;
using NVLearnHub.Application.Interfaces.Services;

namespace NVLearnHub.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private static readonly List<WishlistCourseDto> AvailableCourses = new()
        {
            new WishlistCourseDto
            {
                Id = 1,
                Icon = "🎨",
                IconBg = "rgba(139,92,246,0.2)",
                Category = "Design",
                Title = "UI/UX Design Fundamentals",
                Description = "Learn to craft beautiful, user-centred interfaces from wireframes to polished prototypes.",
                Duration = "6h 30m",
                Lessons = 24
            },
            new WishlistCourseDto
            {
                Id = 2,
                Icon = "⚛️",
                IconBg = "rgba(99,102,241,0.2)",
                Category = "Frontend",
                Title = "React & TypeScript Mastery",
                Description = "Build production-ready apps with React 18, TypeScript, and modern tooling.",
                Duration = "8h 15m",
                Lessons = 30
            },
            new WishlistCourseDto
             {
   Id= 3,
    Icon= "🗄️",
    IconBg= "rgba(34,197,94,0.2)",
    Category= "Backend",
    Title= "Node.js & REST APIs",
    Description= "Design scalable server-side applications and RESTful APIs with Node, Express, and PostgreSQL.",
    Duration= "8h 00m",
    Lessons= 30,
  }
        };

        // shared wishlist (no auth) for now
       // private static readonly List<WishlistCourseDto> _wishlist = new(AvailableCourses);
        private static readonly List<WishlistCourseDto> _wishlist = new();

        public Task<ApiResponse<List<WishlistCourseDto>>> GetWishlistAsync(string? search = null)
        {
            var items = string.IsNullOrWhiteSpace(search)
                ? _wishlist.ToList()
                : _wishlist.Where(c => (c.Title ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)
                                     || (c.Category ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            return Task.FromResult(new ApiResponse<List<WishlistCourseDto>>(items));
        }

        public Task<ApiResponse<WishlistCourseDto>> AddToWishlistAsync(int courseId)
        {
            var course = AvailableCourses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return Task.FromResult(new ApiResponse<WishlistCourseDto>(false, "Course not found."));

            if (_wishlist.Any(c => c.Id == courseId))
                return Task.FromResult(new ApiResponse<WishlistCourseDto>(false, "Course already in wishlist."));

            _wishlist.Add(course);
            return Task.FromResult(new ApiResponse<WishlistCourseDto>(course, "Course added to wishlist."));
        }

        public Task<ApiResponse<bool>> RemoveFromWishlistAsync(int courseId)
        {
            var course = _wishlist.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return Task.FromResult(new ApiResponse<bool>(false, "Course not found in wishlist."));

            _wishlist.Remove(course);
            return Task.FromResult(new ApiResponse<bool>(true, "Course removed from wishlist."));
        }
    }
}
