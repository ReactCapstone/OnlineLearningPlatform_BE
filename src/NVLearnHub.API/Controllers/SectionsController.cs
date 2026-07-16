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
    public class SectionsController : BaseController
    {
        private readonly LearnHubDbContext _context;

        public SectionsController(LearnHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<SectionDto>>>> GetAll()
        {
            var items = await _context.Sections
                .AsNoTracking()
                .Select(s => new SectionDto
                {
                    Id = s.Id,
                    CourseId = s.CourseId,
                    Title = s.Title,
                    OrderIndex = s.OrderIndex
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<SectionDto>>(items));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<SectionDto>>> Get(int id)
        {
            var s = await _context.Sections
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SectionDto
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    Title = x.Title,
                    OrderIndex = x.OrderIndex
                })
                .FirstOrDefaultAsync();

            if (s == null) return NotFound(new ApiResponse<SectionDto>(false, "Section not found."));
            return Ok(new ApiResponse<SectionDto>(s, "Request successful."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SectionDto>>> Create([FromBody] CreateSectionDto dto)
        {
            var section = new Section
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                OrderIndex = dto.OrderIndex
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            var result = new SectionDto
            {
                Id = section.Id,
                CourseId = section.CourseId,
                Title = section.Title,
                OrderIndex = section.OrderIndex
            };

            return CreatedAtAction(nameof(Get), new { id = result.Id }, new ApiResponse<SectionDto>(result, "Section created successfully."));
        }

        [HttpGet("course/{courseId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<SectionDto>>>> GetByCourse(int courseId)
        {
            var items = await _context.Sections
                .AsNoTracking()
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.OrderIndex)
                .Select(x => new SectionDto
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    Title = x.Title,
                    OrderIndex = x.OrderIndex
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<SectionDto>>(items));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] CreateSectionDto dto)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound(new ApiResponse<string>(false, "Section not found."));

            section.CourseId = dto.CourseId;
            section.Title = dto.Title;
            section.OrderIndex = dto.OrderIndex;

            _context.Sections.Update(section);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Section updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound(new ApiResponse<string>(false, "Section not found."));

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Section deleted successfully."));
        }
    }
}
