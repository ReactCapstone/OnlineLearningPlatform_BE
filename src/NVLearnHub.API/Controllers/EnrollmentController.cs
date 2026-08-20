using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NVLearnHub.API.Models;
using NVLearnHub.Domain.Entities.Enrollment;
using NVLearnHub.Infrastructure.Data;
using System.Security.Claims;

namespace NVLearnHub.API.Controllers
{
    [Authorize]
    public class EnrollmentController : BaseController
    {
        private readonly LearnHubDbContext _context;

        public EnrollmentController(LearnHubDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Enroll([FromBody] CreateEnrollmentDto dto)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized(new ApiResponse<EnrollmentDto>(false, "User identity is missing."));

            var courseExists = await _context.Courses.AnyAsync(course => course.Id == dto.CourseId && course.IsPublished);
            if (!courseExists)
                return NotFound(new ApiResponse<EnrollmentDto>(false, "Published course not found."));

            var existing = await _context.Enrollments
                .SingleOrDefaultAsync(enrollment => enrollment.UserId == userId && enrollment.CourseId == dto.CourseId);
            if (existing != null)
            {
                return Ok(new ApiResponse<EnrollmentDto>(ToDto(existing), "Already enrolled in this course."));
            }

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = dto.CourseId,
                Status = "InProgress"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Created(string.Empty, new ApiResponse<EnrollmentDto>(ToDto(enrollment), "Enrollment created successfully."));
        }

        private static EnrollmentDto ToDto(Enrollment enrollment) => new()
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            Status = enrollment.Status
        };
    }
}
