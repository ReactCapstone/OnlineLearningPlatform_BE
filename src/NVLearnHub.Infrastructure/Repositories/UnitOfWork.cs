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

        public UnitOfWork(LearnHubDbContext context, 
            IUserRepository users,
            IRoleRepository roles,
            IPasswordResetTokenRepository passwordResetTokens)
        {
            this._context = context;
            Users = users;
            Roles = roles;
            PasswordResetTokens = passwordResetTokens;
        }


        public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
