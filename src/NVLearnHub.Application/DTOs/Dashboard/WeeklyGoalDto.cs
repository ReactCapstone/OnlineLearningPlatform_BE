namespace NVLearnHub.Application.DTOs.Dashboard
{
    public class WeeklyGoalDto
    {
        public int GoalLessons { get; set; }       // target (e.g. 5)
        public int CompletedThisWeek { get; set; } // how many done this week
        public int ProgressPercent { get; set; }   // CompletedThisWeek / GoalLessons * 100
    }
}