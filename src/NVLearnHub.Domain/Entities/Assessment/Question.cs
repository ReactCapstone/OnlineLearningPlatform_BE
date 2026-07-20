using NVLearnHub.Domain.Common;

namespace NVLearnHub.Domain.Entities.Assessment
{
    public class Question : BaseEntity
    {
        public int AssessmentId { get; set; }
        public string QuestionText { get; set; } = default!;
        public int OrderIndex { get; set; }
        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}