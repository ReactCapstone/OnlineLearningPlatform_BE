using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Identity;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class EmailOtpRepository : Repository<EmailOtp>, IEmailOtpRepository
    {
        public EmailOtpRepository(LearnHubDbContext context) : base(context) { }

        public async Task<EmailOtp?> GetValidOtpAsync(string email, string otpCode)
            => await _context.EmailOtps
                .FirstOrDefaultAsync(o =>
                    o.Email == email &&
                    o.OtpCode == otpCode &&
                    !o.IsVerified &&
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.UtcNow);

        public async Task<EmailOtp?> GetValidVerificationTokenAsync(string verificationToken)
            => await _context.EmailOtps
                .FirstOrDefaultAsync(o =>
                    o.VerificationToken == verificationToken &&
                    o.IsVerified &&
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.UtcNow);

        public async Task InvalidatePreviousOtpsAsync(string email)
        {
            var previous = await _context.EmailOtps
                .Where(o => o.Email == email && !o.IsUsed && !o.IsVerified)
                .ToListAsync();

            foreach (var otp in previous)
                otp.IsUsed = true;
        }

        public async Task<EmailOtp?> GetPendingOtpByEmailAsync(string email)
        => await _context.EmailOtps
            .Where(o => o.Email == email && !o.IsVerified && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }
}