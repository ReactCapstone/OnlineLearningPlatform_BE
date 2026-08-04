using NVLearnHub.Application.DTOs.Dashboard;

namespace NVLearnHub.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(int userId);
        Task<ApiResponse<List<ContinueLearningDto>>> GetContinueLearningAsync(int userId);
        Task<ApiResponse<List<RecommendedCourseDto>>> GetRecommendedCoursesAsync(int userId);
        Task<ApiResponse<List<RecentActivityDto>>> GetRecentActivityAsync(int userId);
        Task<ApiResponse<WeeklyGoalDto>> GetWeeklyGoalAsync(int userId);
        Task<ApiResponse<List<UpcomingClassDto>>> GetUpcomingClassesAsync(int userId);
    }
}