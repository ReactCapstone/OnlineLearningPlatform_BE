using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NVLearnHub.API.Models;
using NVLearnHub.Application.DTOs.Common;
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
                    IsPublished = c.IsPublished
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
                    IsPublished = x.IsPublished
                })
                .FirstOrDefaultAsync();

            if (c == null) return NotFound(new ApiResponse<CourseDto>(false, "Course not found."));
            return Ok(new ApiResponse<CourseDto>(c, "Request successful."));
        }

        [HttpPost]
        [AllowAnonymous]
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
                IsPublished = dto.IsPublished
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
                IsPublished = course.IsPublished
            };

            return CreatedAtAction(nameof(Get), new { id = result.Id }, new ApiResponse<CourseDto>(result, "Course created successfully."));
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] CreateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new ApiResponse<string>(false, "Course not found."));

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Thumbnail = dto.Thumbnail;
            course.CategoryId = dto.CategoryId;
            course.InstructorId = dto.InstructorId;
            course.Price = dto.Price;
            course.Level = dto.Level;
            course.Language = dto.Language;
            course.IsPublished = dto.IsPublished;

            _context.Courses.Update(course);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Course updated successfully."));
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new ApiResponse<string>(false, "Course not found."));

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Course deleted successfully."));
        }
    }
}
