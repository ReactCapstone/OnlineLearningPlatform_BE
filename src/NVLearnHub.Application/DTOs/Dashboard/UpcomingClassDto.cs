namespace NVLearnHub.Application.DTOs.Dashboard
{
    public class UpcomingClassDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Instructor { get; set; } = default!;
        public string DayOfWeek { get; set; } = default!;
        public string Time { get; set; } = default!;
        public DateTime ScheduledAt { get; set; }
    }
}