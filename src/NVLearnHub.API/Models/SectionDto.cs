using System;

namespace NVLearnHub.API.Models
{
    public class SectionDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = default!;
        public int OrderIndex { get; set; }
    }
}
