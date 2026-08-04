using Microsoft.AspNetCore.Authorization;
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

        // ADMIN: create assessment for a course
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<AssessmentDto>>> Create([FromBody] CreateAssessmentDto dto)
        {
            var response = await _assessmentService.CreateAssessmentAsync(dto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetByCourse), new { courseId = dto.CourseId }, response);
        }

        // ADMIN: delete assessment
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _assessmentService.DeleteAssessmentAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Assessment/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<AssessmentDto>>> GetByCourse(int courseId)
        {
            var response = await _assessmentService.GetByCourseAsync(courseId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Assessment/course/{courseId}/my-status
        [HttpGet("course/{courseId}/my-status")]
        public async Task<ActionResult<ApiResponse<AssessmentStatusDto>>> GetStatus(int courseId)
        {
            var userId = GetUserId();
            var response = await _assessmentService.GetStatusAsync(courseId, userId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Assessment/course/{courseId}/attempts
        [HttpGet("course/{courseId}/attempts")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AttemptHistoryDto>>>> GetHistory(int courseId)
        {
            var userId = GetUserId();
            var response = await _assessmentService.GetAttemptHistoryAsync(courseId, userId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // POST /api/Assessment/{id}/start
        [HttpPost("{id}/start")]
        public async Task<ActionResult<ApiResponse<StartAttemptResponseDto>>> StartAttempt(int id)
        {
            var userId = GetUserId();
            var response = await _assessmentService.StartAttemptAsync(id, userId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // POST /api/Assessment/submit
        [HttpPost("submit")]
        public async Task<ActionResult<ApiResponse<AssessmentResultDto>>> Submit([FromBody] SubmitAssessmentDto dto)
        {
            var userId = GetUserId();
            var response = await _assessmentService.SubmitAsync(dto, userId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Assessment/attempts/{attemptId}
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