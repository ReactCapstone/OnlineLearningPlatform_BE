using NVLearnHub.Domain.Entities.Identity;

namespace NVLearnHub.Application.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetValidTokenAsync(string token);
}