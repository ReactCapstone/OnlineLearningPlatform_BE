using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Catalog
{
    public class Lesson : BaseEntity
    {
        public int SectionId { get; set; }
        public Section Section { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string VideoUrl { get; set; } = default!;
        public int DurationSeconds { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPreview { get; set; }
    }
}
