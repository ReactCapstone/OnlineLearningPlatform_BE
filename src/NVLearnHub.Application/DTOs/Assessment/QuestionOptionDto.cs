namespace NVLearnHub.Application.DTOs.Assessment
{
    public class QuestionOptionDto
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = default!;
        // Populated for admin endpoints; null for student-facing responses
        public bool? IsCorrect { get; set; }
    }
}