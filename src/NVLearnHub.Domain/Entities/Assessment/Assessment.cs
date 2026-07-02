using NVLearnHub.Domain.Common;
using NVLearnHub.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Assessment
{
    public class Assessment : BaseEntity
    {
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;
        public string Title { get; set; } = default!;
        public int TimeLimitMinutes { get; set; }
        public int PassPercentage { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
