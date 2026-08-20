using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NVLearnHub.API.Models;
using NVLearnHub.Infrastructure.Data;
using NVLearnHub.Domain.Entities.Enrollment;

namespace NVLearnHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : BaseController
    {
        private readonly LearnHubDbContext _context;

        public ProgressController(LearnHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-progress")]
        public async Task<ActionResult<ApiResponse<MyProgressDto>>> GetMyProgress()
        {
            var userIdValue = User.FindFirst("sub")?.Value;
            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized(new ApiResponse<MyProgressDto>(false, "User identity is missing."));

            var courses = await BuildCourseProgress(userId);
            var overall = courses.Count == 0
                ? 0
                : (int)Math.Round(courses.Average(course => course.ProgressPercentage));

            return Ok(new ApiResponse<MyProgressDto>(new MyProgressDto
            {
                OverallCompletionPercentage = overall,
                Courses = courses
            }, "Progress loaded successfully."));
        }

        [HttpGet("user/{userId}/courses")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseProgressDto>>>> GetUserCourseProgress(int userId)
        {
            var result = await BuildCourseProgress(userId);
            return Ok(new ApiResponse<IEnumerable<CourseProgressDto>>(result));
        }

        private async Task<List<CourseProgressDto>> BuildCourseProgress(int userId)
        {
            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.UserId == userId && e.Status == "InProgress")
                .Include(e => e.Course)
                .ToListAsync();

            var result = new List<CourseProgressDto>();

            foreach (var enrollment in enrollments)
            {
                var courseId = enrollment.CourseId;

                // total lessons in course
                var totalLessons = await _context.Lessons
                    .AsNoTracking()
                    .Where(l => l.Section.CourseId == courseId)
                    .CountAsync();

                // completed lessons for this enrollment
                var completedLessons = await _context.LessonProgress
                    .AsNoTracking()
                    .Where(lp => lp.EnrollmentId == enrollment.Id && lp.IsCompleted)
                    .CountAsync();

                // current lesson: first incomplete lesson ordered by Section.OrderIndex then Lesson.OrderIndex
                var currentLesson = await _context.Lessons
                    .AsNoTracking()
                    .Where(l => l.Section.CourseId == courseId)
                    .OrderBy(l => l.Section.OrderIndex)
                    .ThenBy(l => l.OrderIndex)
                    .FirstOrDefaultAsync(l => !_context.LessonProgress.Any(lp => lp.EnrollmentId == enrollment.Id && lp.LessonId == l.Id && lp.IsCompleted));

                int progressPercent = 0;
                if (totalLessons > 0)
                {
                    progressPercent = (int)Math.Round((double)completedLessons / totalLessons * 100);
                }

                result.Add(new CourseProgressDto
                {
                    EnrollmentId = enrollment.Id,
                    CourseId = courseId,
                    CourseTitle = enrollment.Course.Title,
                    TotalLessons = totalLessons,
                    CompletedLessons = completedLessons,
                    ProgressPercentage = progressPercent,
                    CurrentLessonId = currentLesson?.Id,
                    CurrentLessonTitle = currentLesson?.Title
                });
            }

            return result;
        }

        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LessonProgressDto>>>> GetByEnrollment(int enrollmentId)
        {
            var items = await _context.LessonProgress
                .AsNoTracking()
                .Where(x => x.EnrollmentId == enrollmentId)
                .Select(x => new LessonProgressDto
                {
                    Id = x.Id,
                    EnrollmentId = x.EnrollmentId,
                    LessonId = x.LessonId,
                    IsCompleted = x.IsCompleted,
                    CompletedAt = x.CompletedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<LessonProgressDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LessonProgressDto>>> Get(int id)
        {
            var p = await _context.LessonProgress
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new LessonProgressDto
                {
                    Id = x.Id,
                    EnrollmentId = x.EnrollmentId,
                    LessonId = x.LessonId,
                    IsCompleted = x.IsCompleted,
                    CompletedAt = x.CompletedAt
                })
                .FirstOrDefaultAsync();

            if (p == null) return NotFound(new ApiResponse<LessonProgressDto>(false, "Progress not found."));
            return Ok(new ApiResponse<LessonProgressDto>(p, "Request successful."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<LessonProgressDto>>> Create([FromBody] CreateProgressDto dto)
        {
            // Ensure enrollment exists
            var enrollment = await _context.Enrollments.FindAsync(dto.EnrollmentId);
            if (enrollment == null) return NotFound(new ApiResponse<LessonProgressDto>(false, "Enrollment not found."));

            // Ensure lesson exists
            var lesson = await _context.Lessons.FindAsync(dto.LessonId);
            if (lesson == null) return NotFound(new ApiResponse<LessonProgressDto>(false, "Lesson not found."));

            var existing = await _context.LessonProgress.FirstOrDefaultAsync(x => x.EnrollmentId == dto.EnrollmentId && x.LessonId == dto.LessonId);
            if (existing != null)
            {
                existing.IsCompleted = dto.IsCompleted;
                existing.CompletedAt = dto.IsCompleted ? (dto.CompletedAt ?? DateTime.UtcNow) : null;
                _context.LessonProgress.Update(existing);
                await _context.SaveChangesAsync();

                var res = new LessonProgressDto
                {
                    Id = existing.Id,
                    EnrollmentId = existing.EnrollmentId,
                    LessonId = existing.LessonId,
                    IsCompleted = existing.IsCompleted,
                    CompletedAt = existing.CompletedAt
                };

                return Ok(new ApiResponse<LessonProgressDto>(res, "Progress updated."));
            }

            var progress = new LessonProgress
            {
                EnrollmentId = dto.EnrollmentId,
                LessonId = dto.LessonId,
                IsCompleted = dto.IsCompleted,
                CompletedAt = dto.IsCompleted ? (dto.CompletedAt ?? DateTime.UtcNow) : null
            };

            _context.LessonProgress.Add(progress);
            await _context.SaveChangesAsync();

            var result = new LessonProgressDto
            {
                Id = progress.Id,
                EnrollmentId = progress.EnrollmentId,
                LessonId = progress.LessonId,
                IsCompleted = progress.IsCompleted,
                CompletedAt = progress.CompletedAt
            };

            return CreatedAtAction(nameof(Get), new { id = result.Id }, new ApiResponse<LessonProgressDto>(result, "Progress created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] CreateProgressDto dto)
        {
            var progress = await _context.LessonProgress.FindAsync(id);
            if (progress == null) return NotFound(new ApiResponse<string>(false, "Progress not found."));

            progress.IsCompleted = dto.IsCompleted;
            progress.CompletedAt = dto.IsCompleted ? (dto.CompletedAt ?? DateTime.UtcNow) : null;

            _context.LessonProgress.Update(progress);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Progress updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var progress = await _context.LessonProgress.FindAsync(id);
            if (progress == null) return NotFound(new ApiResponse<string>(false, "Progress not found."));

            _context.LessonProgress.Remove(progress);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Progress deleted successfully."));
        }
    }
}
