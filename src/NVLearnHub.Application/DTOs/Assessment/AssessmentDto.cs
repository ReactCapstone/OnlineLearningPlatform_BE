namespace NVLearnHub.Application.DTOs.Assessment
{
    public class AssessmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int TimeLimitMinutes { get; set; }
        public int PassPercentage { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }
}