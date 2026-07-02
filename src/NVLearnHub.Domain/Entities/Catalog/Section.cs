using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Catalog
{
    public class Section : BaseEntity
    {
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;
        public string Title { get; set; } = default!;
        public int OrderIndex { get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
