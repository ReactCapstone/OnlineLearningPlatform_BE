using System;

namespace NVLearnHub.API.Models
{
    public class CreateLessonDto
    {
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPreview { get; set; }
    }
}
