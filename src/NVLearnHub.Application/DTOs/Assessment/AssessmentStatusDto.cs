namespace NVLearnHub.Application.DTOs.Assessment
{
    public class AssessmentStatusDto
    {
        public bool HasAttempted { get; set; }
        public int? AttemptId { get; set; }
        public AssessmentResultDto? Result { get; set; }
    }
}