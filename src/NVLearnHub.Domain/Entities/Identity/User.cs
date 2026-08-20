using NVLearnHub.Domain.Common;
using System.Collections.Generic;

namespace NVLearnHub.Domain.Entities.Identity
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;
        public bool IsActive { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? AreaOfExpertise { get; set; }
        public List<string> Expertise { get; set; } = new();
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    }
}