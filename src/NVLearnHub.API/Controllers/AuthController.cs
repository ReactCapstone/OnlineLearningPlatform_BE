using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NVLearnHub.Application.DTOs.Auth;
using NVLearnHub.Application.Interfaces.Services;

namespace NVLearnHub.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            if (!response.Success)
                return Unauthorized(response);
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<ForgotPasswordResponseDto>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var response = await _authService.ForgotPasswordAsync(dto);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<ResetPasswordResponseDto>>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var response = await _authService.ResetPasswordAsync(dto);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}