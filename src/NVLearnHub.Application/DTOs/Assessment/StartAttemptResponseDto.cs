namespace NVLearnHub.Application.DTOs.Assessment
{
    public class StartAttemptResponseDto
    {
        public int AttemptId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime ExpiresAt { get; set; }   // StartedAt + TimeLimitMinutes
        public int TimeLimitMinutes { get; set; }
    }
}