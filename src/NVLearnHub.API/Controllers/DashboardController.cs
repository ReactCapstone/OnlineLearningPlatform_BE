using Microsoft.AspNetCore.Mvc;
using NVLearnHub.Application.DTOs.Dashboard;
using NVLearnHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace NVLearnHub.API.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET /api/Dashboard/stats
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
        {
            var response = await _dashboardService.GetStatsAsync(GetUserId());
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Dashboard/continue-learning
        [HttpGet("continue-learning")]
        public async Task<ActionResult<ApiResponse<List<ContinueLearningDto>>>> GetContinueLearning()
        {
            var response = await _dashboardService.GetContinueLearningAsync(GetUserId());
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Dashboard/recommended
        [HttpGet("recommended")]
        public async Task<ActionResult<ApiResponse<List<RecommendedCourseDto>>>> GetRecommended()
        {
            var response = await _dashboardService.GetRecommendedCoursesAsync(GetUserId());
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Dashboard/recent-activity
        [HttpGet("recent-activity")]
        public async Task<ActionResult<ApiResponse<List<RecentActivityDto>>>> GetRecentActivity()
        {
            var response = await _dashboardService.GetRecentActivityAsync(GetUserId());
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Dashboard/weekly-goal
        [HttpGet("weekly-goal")]
        public async Task<ActionResult<ApiResponse<WeeklyGoalDto>>> GetWeeklyGoal()
        {
            var response = await _dashboardService.GetWeeklyGoalAsync(GetUserId());
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Dashboard/upcoming-classes
        [HttpGet("upcoming-classes")]
        public async Task<ActionResult<ApiResponse<List<UpcomingClassDto>>>> GetUpcomingClasses()
        {
            var response = await _dashboardService.GetUpcomingClassesAsync(GetUserId());
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        private int GetUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub")!);
    }
}