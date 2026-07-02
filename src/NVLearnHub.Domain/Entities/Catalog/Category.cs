using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Catalog
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
