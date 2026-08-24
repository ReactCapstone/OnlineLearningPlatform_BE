namespace NVLearnHub.Application.DTOs.Auth
{
    public class VerifyOtpDto
    {
        public string Email { get; set; } = default!;
        public string OtpCode { get; set; } = default!;
    }
}