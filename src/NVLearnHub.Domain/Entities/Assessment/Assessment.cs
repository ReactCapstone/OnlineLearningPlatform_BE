using NVLearnHub.Domain.Common;
using NVLearnHub.Domain.Entities.Assessment;
using NVLearnHub.Domain.Entities.Catalog;

public class Assessment : BaseEntity
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int TimeLimitMinutes { get; set; }
    public int PassPercentage { get; set; }
    public int MaxAttempts { get; set; } = 3; // ← default 3, 0 = unlimited

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<AssessmentAttempt> Attempts { get; set; } = new List<AssessmentAttempt>();
}