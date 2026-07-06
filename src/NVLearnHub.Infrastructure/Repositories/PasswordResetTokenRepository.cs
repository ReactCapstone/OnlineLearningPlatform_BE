using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Identity;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories;

public class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(LearnHubDbContext context) : base(context) { }

    public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        => await _context.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == token
                                   && !t.IsUsed
                                   && t.ExpiresAt > DateTime.UtcNow);
}