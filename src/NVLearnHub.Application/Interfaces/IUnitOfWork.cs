using NVLearnHub.Application.Interfaces.Repositories;

namespace NVLearnHub.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Identity
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IPasswordResetTokenRepository PasswordResetTokens { get; }

    Task<int> SaveChangesAsync();
}