using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LearnHubDbContext _context;

        public IUserRepository Users { get; }

        public IRoleRepository Roles {  get; }

        public IPasswordResetTokenRepository PasswordResetTokens {  get; }

        public UnitOfWork(LearnHubDbContext context)
        {
            this._context = context;
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            PasswordResetTokens = new PasswordResetTokenRepository(_context);
        }


        public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
