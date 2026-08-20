using NVLearnHub.Domain.Common;

namespace NVLearnHub.Domain.Entities.Catalog
{
    public class Goal : BaseEntity
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string RequiredSkill { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}
