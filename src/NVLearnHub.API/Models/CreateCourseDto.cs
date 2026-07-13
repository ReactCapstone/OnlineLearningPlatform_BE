using System;

namespace NVLearnHub.API.Models
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Thumbnail { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int InstructorId { get; set; }
        public decimal Price { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
    }
}
