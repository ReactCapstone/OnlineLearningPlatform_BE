using Microsoft.AspNetCore.Mvc;
using NVLearnHub.Application.DTOs.Wishlist;
using NVLearnHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace NVLearnHub.API.Controllers
{
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET /api/Wishlist
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<WishlistCourseDto>>>> Get([FromQuery] string? search)
        {
            var userId = GetUserId();
            var response = await _wishlistService.GetWishlistAsync(userId, search);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // POST /api/Wishlist/{courseId}
        [HttpPost("{courseId:int}")]
        public async Task<ActionResult<ApiResponse<WishlistCourseDto>>> Add(int courseId)
        {
            var userId = GetUserId();
            var response = await _wishlistService.AddToWishlistAsync(userId, courseId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // DELETE /api/Wishlist/{courseId}
        [HttpDelete("{courseId:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Remove(int courseId)
        {
            var userId = GetUserId();
            var response = await _wishlistService.RemoveFromWishlistAsync(userId, courseId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // ─── Helper ──────────────────────────────────────────────────────────
        private int GetUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub")!);
    }
}