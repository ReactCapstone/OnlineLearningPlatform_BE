namespace NVLearnHub.Application.DTOs.Dashboard
{
    public class RecentActivityDto
    {
        public string ActivityType { get; set; } = default!; // "LessonCompleted", "QuizSubmitted", "CertificateEarned"
        public string Title { get; set; } = default!;
        public DateTime ActivityDate { get; set; }
        public string TimeAgo { get; set; } = default!;
    }
}