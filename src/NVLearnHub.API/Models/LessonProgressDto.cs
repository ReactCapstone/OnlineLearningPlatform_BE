using System;

namespace NVLearnHub.API.Models
{
    public class LessonProgressDto
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
