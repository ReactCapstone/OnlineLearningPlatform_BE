using NVLearnHub.Domain.Common;

namespace NVLearnHub.Domain.Entities.Identity
{
    public class EmailOtp : BaseEntity
    {
        public string Email { get; set; } = default!;
        public string OtpCode { get; set; } = default!;
        public string? VerificationToken { get; set; }  // set after OTP verified
        public bool IsVerified { get; set; }
        public bool IsUsed { get; set; }               // set after full register
        public DateTime ExpiresAt { get; set; }
    }
}