using System;

namespace NVLearnHub.API.Models
{
    public class LessonDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = default!;
        public string VideoUrl { get; set; } = default!;
        public int DurationSeconds { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPreview { get; set; }
    }
}
