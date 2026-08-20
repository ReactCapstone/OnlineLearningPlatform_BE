using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using NVLearnHub.API.Models;
using NVLearnHub.Infrastructure.Data;
using NVLearnHub.Domain.Entities.Catalog;

namespace NVLearnHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : BaseController
    {
        private readonly LearnHubDbContext _context;

        /*
        // Optional hardcoded seed data. Uncomment to use local seed instead of DB.
        private static readonly List<CourseDto> _seedCourses = new()
        {
            new CourseDto { Id = 1, Title = "Intro to C#", Description = "Basics of C# programming.", Thumbnail = "", CategoryId = 1, InstructorId = 1, Price = 0m, Level = "Beginner", Language = "English", IsPublished = true },
            new CourseDto { Id = 2, Title = "ASP.NET Core Web API", Description = "Build APIs with ASP.NET Core.", Thumbnail = "", CategoryId = 1, InstructorId = 2, Price = 9.99m, Level = "Intermediate", Language = "English", IsPublished = true },
            new CourseDto { Id = 3, Title = "Entity Framework Core", Description = "Data access with EF Core.", Thumbnail = "", CategoryId = 1, InstructorId = 2, Price = 14.99m, Level = "Intermediate", Language = "English", IsPublished = false }
        };
        */

        public CoursesController(LearnHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseDto>>>> GetAll()
        {
            var items = await _context.Courses
                .AsNoTracking()
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Thumbnail = c.Thumbnail,
                    CategoryId = c.CategoryId,
                    InstructorId = c.InstructorId,
                    Price = c.Price,
                    Level = c.Level,
                    Language = c.Language,
                    IsPublished = c.IsPublished,
                    NumberOfLessons = c.NumberOfLessons
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<CourseDto>>(items));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Get(int id)
        {
            var c = await _context.Courses
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CourseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Thumbnail = x.Thumbnail,
                    CategoryId = x.CategoryId,
                    InstructorId = x.InstructorId,
                    Price = x.Price,
                    Level = x.Level,
                    Language = x.Language,
                    IsPublished = x.IsPublished,
                    NumberOfLessons = x.NumberOfLessons
                })
                .FirstOrDefaultAsync();

            if (c == null) return NotFound(new ApiResponse<CourseDto>(false, "Course not found."));
            return Ok(new ApiResponse<CourseDto>(c, "Request successful."));
        }

        /// <summary>
        /// Creates a new course as either a draft or published record.
        /// </summary>
        /// <param name="dto">Course creation DTO.</param>
        /// <returns>Created course DTO or validation error.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Create([FromBody] CreateCourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Thumbnail = dto.Thumbnail,
                CategoryId = dto.CategoryId,
                InstructorId = dto.InstructorId,
                Price = dto.Price,
                Level = dto.Level,
                Language = dto.Language,
                IsPublished = dto.IsPublished,
                NumberOfLessons = dto.NumberOfLessons
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Thumbnail = course.Thumbnail,
                CategoryId = course.CategoryId,
                InstructorId = course.InstructorId,
                Price = course.Price,
                Level = course.Level,
                Language = course.Language,
                IsPublished = course.IsPublished,
                NumberOfLessons = course.NumberOfLessons
            };

            return CreatedAtAction(nameof(Get), new { id = result.Id }, new ApiResponse<CourseDto>(result, "Course created successfully."));
        }

        /// <summary>
        /// Updates an existing course. When publishing (IsPublished=true) validates that configured NumberOfLessons matches actual lessons linked to the course.
        /// </summary>
        /// <param name="id">Course id</param>
        /// <param name="dto">Course DTO</param>
        /// <returns>Result message or validation error</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] CreateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new ApiResponse<string>(false, "Course not found."));

            // Basic server-side validation
            if (dto.NumberOfLessons < 0)
            {
                return BadRequest(new ApiResponse<string>(false, "NumberOfLessons cannot be negative."));
            }

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Thumbnail = dto.Thumbnail;
            course.CategoryId = dto.CategoryId;
            course.InstructorId = dto.InstructorId;
            course.Price = dto.Price;
            course.Level = dto.Level;
            course.Language = dto.Language;
            course.IsPublished = dto.IsPublished;
            course.NumberOfLessons = dto.NumberOfLessons;

            // If trying to publish, validate counts
            if (dto.IsPublished)
            {
                var actualCount = await _context.Lessons
                    .Include(l => l.Section)
                    .Where(l => l.Section != null && l.Section.CourseId == course.Id)
                    .CountAsync();

                if (dto.NumberOfLessons != actualCount)
                {
                    return BadRequest(new ApiResponse<string>(false, $"Number of lessons does not match. Configured: {dto.NumberOfLessons}, Actual: {actualCount}"));
                }
            }

            _context.Courses.Update(course);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Course updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            // Load course along with sections and lessons
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound(new ApiResponse<string>(false, "Course not found."));

            // Remove lessons and sections
            var lessons = course.Sections.SelectMany(s => s.Lessons).ToList();
            if (lessons.Any()) _context.Lessons.RemoveRange(lessons);

            if (course.Sections.Any()) _context.Sections.RemoveRange(course.Sections);

            // Remove assessments and their related data (questions/options/attempts/answers)
            var assessments = await _context.Assessments
                .Where(a => a.CourseId == id)
                .ToListAsync();

            if (assessments.Any())
            {
                // Load attempts and answers for all assessments in one go
                var assessmentIds = assessments.Select(a => a.Id).ToList();

                var attempts = await _context.AssessmentAttempts
                    .Where(at => assessmentIds.Contains(at.AssessmentId))
                    .ToListAsync();

                var attemptIds = attempts.Select(a => a.Id).ToList();

                if (attemptIds.Any())
                {
                    var answers = await _context.AssessmentAnswers
                        .Where(ans => attemptIds.Contains(ans.AttemptId))
                        .ToListAsync();
                    if (answers.Any()) _context.AssessmentAnswers.RemoveRange(answers);

                    _context.AssessmentAttempts.RemoveRange(attempts);
                }

                // Load questions and options for these assessments and remove
                var questions = await _context.Questions
                    .Where(q => assessmentIds.Contains(q.AssessmentId))
                    .Include(q => q.Options)
                    .ToListAsync();

                var options = questions.SelectMany(q => q.Options).ToList();
                if (options.Any()) _context.QuestionOptions.RemoveRange(options);
                if (questions.Any()) _context.Questions.RemoveRange(questions);

                // Remove assessments
                _context.Assessments.RemoveRange(assessments);
            }

            // Finally remove the course
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Course deleted successfully."));
        }
    }
}
