using NVLearnHub.Domain.Common;
using NVLearnHub.Domain.Entities.Catalog;
using NVLearnHub.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Enrollment
{
    public class Wishlist : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;
    }
}
