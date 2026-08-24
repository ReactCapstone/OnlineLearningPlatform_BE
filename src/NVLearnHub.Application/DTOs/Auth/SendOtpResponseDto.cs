namespace NVLearnHub.Application.DTOs.Auth
{
    public class SendOtpResponseDto
    {
        public string Email { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
    }
}