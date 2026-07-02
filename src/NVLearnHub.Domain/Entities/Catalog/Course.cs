using NVLearnHub.Domain.Common;
using NVLearnHub.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Catalog
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Thumbnail { get; set; } = default!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public int InstructorId { get; set; }
        public User Instructor { get; set; } = default!;
        public decimal Price { get; set; }
        public string Level { get; set; } = default!;
        public string Language { get; set; } = default!;
        public bool IsPublished { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
