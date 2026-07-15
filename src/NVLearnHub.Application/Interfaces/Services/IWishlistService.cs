using NVLearnHub.Application.DTOs.Common;
using NVLearnHub.Application.DTOs.Wishlist;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NVLearnHub.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<ApiResponse<List<WishlistCourseDto>>> GetWishlistAsync(string? search = null);
        Task<ApiResponse<WishlistCourseDto>> AddToWishlistAsync(int courseId);
        Task<ApiResponse<bool>> RemoveFromWishlistAsync(int courseId);
    }
}
