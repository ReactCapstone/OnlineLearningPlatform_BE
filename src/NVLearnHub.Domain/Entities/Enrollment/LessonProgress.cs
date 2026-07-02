using NVLearnHub.Domain.Common;
using NVLearnHub.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Enrollment
{
    public class LessonProgress : BaseEntity
    {
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = default!;
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = default!;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
