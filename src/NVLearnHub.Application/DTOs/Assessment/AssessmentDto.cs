namespace NVLearnHub.Application.DTOs.Assessment
{
    public class AssessmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int TimeLimitMinutes { get; set; }
        public int PassPercentage { get; set; }
        public int TotalQuestions { get; set; }
        public int MaxAttempts { get; set; }      // ← 0 means unlimited
        public List<QuestionDto> Questions { get; set; } = new();
    }
}