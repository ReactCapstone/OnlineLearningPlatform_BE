using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NVLearnHub.Application.DTOs.Common;
using NVLearnHub.Application.DTOs.Wishlist;
using NVLearnHub.Application.Interfaces.Services;

namespace NVLearnHub.API.Controllers
{
    [AllowAnonymous]
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<WishlistCourseDto>>>> Get([FromQuery] string? search)
        {
            var response = await _wishlistService.GetWishlistAsync(search);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("{courseId:guid}")]
        public async Task<ActionResult<ApiResponse<WishlistCourseDto>>> Add(System.Guid courseId)
        {
            var response = await _wishlistService.AddToWishlistAsync(courseId);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{courseId:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Remove(System.Guid courseId)
        {
            var response = await _wishlistService.RemoveFromWishlistAsync(courseId);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }
    }
}
