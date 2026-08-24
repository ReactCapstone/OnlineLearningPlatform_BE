namespace NVLearnHub.Application.DTOs.Auth
{
    public class VerifyOtpResponseDto
    {
        public string VerificationToken { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
    }
}