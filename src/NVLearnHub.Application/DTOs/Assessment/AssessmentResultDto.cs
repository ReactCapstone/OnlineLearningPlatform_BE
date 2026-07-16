namespace NVLearnHub.Application.DTOs.Assessment
{
    public class AssessmentResultDto
    {
        public int AttemptId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int PassPercentage { get; set; }
        public bool IsPassed { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}