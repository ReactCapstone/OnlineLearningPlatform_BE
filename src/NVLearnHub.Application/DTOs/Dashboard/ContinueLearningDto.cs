namespace NVLearnHub.Application.DTOs.Dashboard
{
    public class ContinueLearningDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = default!;
        public string Instructor { get; set; } = default!;
        public string Thumbnail { get; set; } = default!;
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int ProgressPercent { get; set; }
        public string? CurrentLesson { get; set; }
    }
}