namespace NVLearnHub.Application.DTOs.Dashboard
{
    public class RecommendedCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Instructor { get; set; } = default!;
        public string Thumbnail { get; set; } = default!;
        public string Category { get; set; } = default!;
        public string Level { get; set; } = default!;
        public decimal Price { get; set; }
        public int TotalLessons { get; set; }
    }
}