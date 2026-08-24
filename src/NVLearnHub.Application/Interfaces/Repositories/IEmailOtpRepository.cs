using NVLearnHub.Domain.Entities.Identity;

namespace NVLearnHub.Application.Interfaces.Repositories
{
    public interface IEmailOtpRepository : IRepository<EmailOtp>
    {
        Task<EmailOtp?> GetValidOtpAsync(string email, string otpCode);
        Task<EmailOtp?> GetValidVerificationTokenAsync(string verificationToken);
        Task InvalidatePreviousOtpsAsync(string email);
        Task<EmailOtp?> GetPendingOtpByEmailAsync(string email);
    }
}