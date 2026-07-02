using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using NVLearnHub.Domain.Common;

namespace NVLearnHub.Domain.Entities.Identity
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
