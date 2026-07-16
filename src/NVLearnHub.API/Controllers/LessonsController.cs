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
    public class LessonsController : BaseController
    {
        private readonly LearnHubDbContext _context;

        public LessonsController(LearnHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<LessonDto>>>> GetAll()
        {
            var items = await _context.Lessons
                .AsNoTracking()
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    SectionId = l.SectionId,
                    Title = l.Title,
                    VideoUrl = l.VideoUrl,
                    DurationSeconds = l.DurationSeconds,
                    OrderIndex = l.OrderIndex,
                    IsPreview = l.IsPreview
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<LessonDto>>(items));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LessonDto>>> Get(int id)
        {
            var l = await _context.Lessons
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new LessonDto
                {
                    Id = x.Id,
                    SectionId = x.SectionId,
                    Title = x.Title,
                    VideoUrl = x.VideoUrl,
                    DurationSeconds = x.DurationSeconds,
                    OrderIndex = x.OrderIndex,
                    IsPreview = x.IsPreview
                })
                .FirstOrDefaultAsync();

            if (l == null) return NotFound(new ApiResponse<LessonDto>(false, "Lesson not found."));
            return Ok(new ApiResponse<LessonDto>(l, "Request successful."));
        }
        [HttpGet("section/{sectionId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<LessonDto>>>> GetBySection(int sectionId)
        {
            var lessons = await _context.Lessons
                .AsNoTracking()
                .Where(x => x.SectionId == sectionId)
                .OrderBy(x => x.OrderIndex)
                .Select(x => new LessonDto
                {
                    Id = x.Id,
                    SectionId = x.SectionId,
                    Title = x.Title,
                    VideoUrl = x.VideoUrl,
                    DurationSeconds = x.DurationSeconds,
                    OrderIndex = x.OrderIndex,
                    IsPreview = x.IsPreview
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<LessonDto>>(lessons));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<LessonDto>>> Create([FromBody] CreateLessonDto dto)
        {
            var lesson = new Lesson
            {
                SectionId = dto.SectionId,
                Title = dto.Title,
                VideoUrl = dto.VideoUrl,
                DurationSeconds = dto.DurationSeconds,
                OrderIndex = dto.OrderIndex,
                IsPreview = dto.IsPreview
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            var result = new LessonDto
            {
                Id = lesson.Id,
                SectionId = lesson.SectionId,
                Title = lesson.Title,
                VideoUrl = lesson.VideoUrl,
                DurationSeconds = lesson.DurationSeconds,
                OrderIndex = lesson.OrderIndex,
                IsPreview = lesson.IsPreview
            };

            return CreatedAtAction(nameof(Get), new { id = result.Id }, new ApiResponse<LessonDto>(result, "Lesson created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] CreateLessonDto dto)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound(new ApiResponse<string>(false, "Lesson not found."));

            lesson.SectionId = dto.SectionId;
            lesson.Title = dto.Title;
            lesson.VideoUrl = dto.VideoUrl;
            lesson.DurationSeconds = dto.DurationSeconds;
            lesson.OrderIndex = dto.OrderIndex;
            lesson.IsPreview = dto.IsPreview;

            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Lesson updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound(new ApiResponse<string>(false, "Lesson not found."));

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Lesson deleted successfully."));
        }
    }
}
