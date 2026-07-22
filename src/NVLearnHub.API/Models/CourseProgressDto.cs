namespace NVLearnHub.API.Models
{
    public class CourseProgressDto
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = default!;
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int ProgressPercentage { get; set; }
        public int? CurrentLessonId { get; set; }
        public string? CurrentLessonTitle { get; set; }
    }
}
