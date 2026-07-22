using System;

namespace NVLearnHub.API.Models
{
    public class CreateProgressDto
    {
        public int EnrollmentId { get; set; }
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
