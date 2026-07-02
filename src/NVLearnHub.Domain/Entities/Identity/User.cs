using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using NVLearnHub.Domain.Common;
namespace NVLearnHub.Domain.Entities.Identity
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;
        public bool IsActive { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    }
}
