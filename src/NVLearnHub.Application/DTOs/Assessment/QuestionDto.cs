namespace NVLearnHub.Application.DTOs.Assessment
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = default!;
        public int OrderIndex { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }
}