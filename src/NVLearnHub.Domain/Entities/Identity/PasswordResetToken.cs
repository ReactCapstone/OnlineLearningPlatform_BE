using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Identity
{
    public class PasswordResetToken : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
