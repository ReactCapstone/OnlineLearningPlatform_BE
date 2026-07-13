using NVLearnHub.Application.DTOs.Common;
using NVLearnHub.Application.DTOs.Wishlist;
using NVLearnHub.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVLearnHub.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private static readonly List<WishlistCourseDto> AvailableCourses = new()
        {
            new WishlistCourseDto
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "UI/UX Design Fundamentals",
                Description = "Learn to craft beautiful, user-centred interfaces, from wireframes to polished prototypes.",
                Category = "Design",
                ImageUrl = "https://example.com/images/uiux.png",
                Instructor = "Jane Doe",
                Price = 49.99m
            },
            new WishlistCourseDto
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "React & TypeScript Mastery",
                Description = "Build production-ready apps with React 18, TypeScript, and modern tooling.",
                Category = "Frontend",
                ImageUrl = "https://example.com/images/react-ts.png",
                Instructor = "John Smith",
                Price = 59.99m
            }
        };

        // shared wishlist (no auth) for now
        private static readonly List<WishlistCourseDto> _wishlist = new(AvailableCourses);

        public Task<ApiResponse<List<WishlistCourseDto>>> GetWishlistAsync(string? search = null)
        {
            var items = string.IsNullOrWhiteSpace(search)
                ? _wishlist.ToList()
                : _wishlist.Where(c => (c.Title ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)
                                     || (c.Category ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            return Task.FromResult(new ApiResponse<List<WishlistCourseDto>>(items));
        }

        public Task<ApiResponse<WishlistCourseDto>> AddToWishlistAsync(Guid courseId)
        {
            var course = AvailableCourses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return Task.FromResult(new ApiResponse<WishlistCourseDto>(false, "Course not found."));

            if (_wishlist.Any(c => c.Id == courseId))
                return Task.FromResult(new ApiResponse<WishlistCourseDto>(false, "Course already in wishlist."));

            _wishlist.Add(course);
            return Task.FromResult(new ApiResponse<WishlistCourseDto>(course, "Course added to wishlist."));
        }

        public Task<ApiResponse<bool>> RemoveFromWishlistAsync(Guid courseId)
        {
            var course = _wishlist.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return Task.FromResult(new ApiResponse<bool>(false, "Course not found in wishlist."));

            _wishlist.Remove(course);
            return Task.FromResult(new ApiResponse<bool>(true, "Course removed from wishlist."));
        }
    }
}
