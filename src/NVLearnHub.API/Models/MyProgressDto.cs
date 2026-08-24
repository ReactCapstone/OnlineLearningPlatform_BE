namespace NVLearnHub.API.Models
{
    public class MyProgressDto
    {
        public int OverallCompletionPercentage { get; set; }
        public IEnumerable<CourseProgressDto> Courses { get; set; } = Array.Empty<CourseProgressDto>();
    }
}
