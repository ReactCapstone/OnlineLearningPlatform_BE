namespace NVLearnHub.Application.DTOs.Assessment;

public class CreateAssessmentDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = default!;
    public int TimeLimitMinutes { get; set; }
    public int PassPercentage { get; set; }
    public int MaxAttempts { get; set; }
    public List<CreateQuestionDto> Questions { get; set; } = new();
}

public class CreateQuestionDto
{
    public string QuestionText { get; set; } = default!;
    public int OrderIndex { get; set; }
    public List<CreateQuestionOptionDto> Options { get; set; } = new();
}

public class CreateQuestionOptionDto
{
    public string OptionText { get; set; } = default!;
    public bool IsCorrect { get; set; }
}
