using Microsoft.AspNetCore.Mvc;
using NVLearnHub.Application.DTOs.Assessment;
using NVLearnHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace NVLearnHub.API.Controllers
{
    public class AssessmentController : BaseController
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        // GET /api/assessment/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<AssessmentDto>>> GetByCourse(int courseId)
        {
            var response = await _assessmentService.GetByCourseAsync(courseId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // POST /api/assessment/{id}/start
        [HttpPost("{id}/start")]
        public async Task<ActionResult<ApiResponse<StartAttemptResponseDto>>> StartAttempt(int id)
        {
            var userId = GetUserId();
            var response = await _assessmentService.StartAttemptAsync(id, userId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // POST /api/assessment/submit
        [HttpPost("submit")]
        public async Task<ActionResult<ApiResponse<AssessmentResultDto>>> Submit([FromBody] SubmitAssessmentDto dto)
        {
            var userId = GetUserId();
            var response = await _assessmentService.SubmitAsync(dto, userId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/assessment/attempts/{attemptId}
        [HttpGet("attempts/{attemptId}")]
        public async Task<ActionResult<ApiResponse<AssessmentResultDto>>> GetResult(int attemptId)
        {
            var userId = GetUserId();
            var response = await _assessmentService.GetAttemptResultAsync(attemptId, userId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // ─── Helper ──────────────────────────────────────────────────────────
        private int GetUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub")!);
    }
}