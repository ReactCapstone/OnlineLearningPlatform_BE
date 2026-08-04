namespace NVLearnHub.Application.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int EnrolledCourses { get; set; }
        public int InProgressCourses { get; set; }
        public int CompletedCourses { get; set; }
        public int CertificatesEarned { get; set; }
    }
}