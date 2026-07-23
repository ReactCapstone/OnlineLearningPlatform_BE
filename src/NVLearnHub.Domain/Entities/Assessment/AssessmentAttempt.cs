using NVLearnHub.Domain.Common;
using NVLearnHub.Domain.Entities.Identity;

namespace NVLearnHub.Domain.Entities.Assessment
{
    public class AssessmentAttempt : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public int AssessmentId { get; set; }
        public int Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public ICollection<AssessmentAnswer> Answers { get; set; } = new List<AssessmentAnswer>();
    }
}