namespace NVLearnHub.Application.DTOs.Assessment;

public class CreateAssessmentDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = default!;
    public int TimeLimitMinutes { get; set; }
    public int PassPercentage { get; set; }
    public int MaxAttempts { get; set; }
}
