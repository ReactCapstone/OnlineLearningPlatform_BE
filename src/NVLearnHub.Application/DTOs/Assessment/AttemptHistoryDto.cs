namespace NVLearnHub.Application.DTOs.Assessment
{
    public class AttemptHistoryDto
    {
        public int AttemptId { get; set; }
        public int Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}