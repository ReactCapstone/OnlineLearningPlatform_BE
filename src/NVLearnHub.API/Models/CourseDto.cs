using System;

namespace NVLearnHub.API.Models
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Thumbnail { get; set; } = default!;
        public int CategoryId { get; set; }
        public int InstructorId { get; set; }
        public decimal Price { get; set; }
        public string Level { get; set; } = default!;
        public string Language { get; set; } = default!;
        public bool IsPublished { get; set; }
    }
}
