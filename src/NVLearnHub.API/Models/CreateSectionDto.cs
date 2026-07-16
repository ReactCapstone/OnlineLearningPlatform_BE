using System;

namespace NVLearnHub.API.Models
{
    public class CreateSectionDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}
