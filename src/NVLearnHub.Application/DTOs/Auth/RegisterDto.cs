namespace NVLearnHub.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public int? YearsOfExperience { get; set; }
        public string? AreaOfExpertise { get; set; }
    }
}