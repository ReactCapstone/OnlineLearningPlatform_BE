namespace NVLearnHub.API.Models
{
    public class GoalDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string RequiredSkill { get; set; } = default!;
    }
}
