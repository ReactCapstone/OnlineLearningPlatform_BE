namespace NVLearnHub.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public int? YearsOfExperience { get; set; }
        public string? AreaOfExpertise { get; set; }
        public List<string> Expertise { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
    }
}