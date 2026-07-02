using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Admin
{
    public class Advertisement : BaseEntity
    {
        public string Title { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public string LinkUrl { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
