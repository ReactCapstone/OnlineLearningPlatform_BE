namespace NVLearnHub.Application.DTOs.Assessment
{
    public class SubmitAssessmentDto
    {
        public int AttemptId { get; set; }
        public List<SubmitAnswerDto> Answers { get; set; } = new();
    }
}