using NVLearnHub.Application.DTOs.Wishlist;

namespace NVLearnHub.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<ApiResponse<List<WishlistCourseDto>>> GetWishlistAsync(int userId, string? search = null);
        Task<ApiResponse<WishlistCourseDto>> AddToWishlistAsync(int userId, int courseId);
        Task<ApiResponse<bool>> RemoveFromWishlistAsync(int userId, int courseId);
    }
}