using NVLearnHub.Application.DTOs.Auth;
using NVLearnHub.Application.DTOs.Common;

namespace NVLearnHub.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ApiResponse<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ApiResponse<ResetPasswordResponseDto>> ResetPasswordAsync(ResetPasswordDto dto);
    }
}