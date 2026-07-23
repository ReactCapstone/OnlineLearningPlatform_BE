namespace NVLearnHub.Application.DTOs.Assessment
{
    public class AssessmentStatusDto
    {
        public bool HasActiveAttempt { get; set; }
        public int? ActiveAttemptId { get; set; }
        public AssessmentResultDto? LatestResult { get; set; }
        public int AttemptCount { get; set; }
        public int MaxAttempts { get; set; }        // 0 = unlimited
        public int? AttemptsRemaining { get; set; } // null if unlimited
        public bool CanRetake { get; set; }         // false if limit reached
    }
}