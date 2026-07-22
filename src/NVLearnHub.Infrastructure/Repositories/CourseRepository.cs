using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Catalog;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(LearnHubDbContext context) : base(context) { }

        public async Task<Course?> GetWithSectionsAndLessonsAsync(int courseId)
            => await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Sections.OrderBy(s => s.OrderIndex))
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .FirstOrDefaultAsync(c => c.Id == courseId);

        public async Task<IEnumerable<Course>> SearchAsync(string? keyword, int? categoryId,
                                                            string? level, string? sortBy)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Where(c => c.IsPublished);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(c => c.Title.Contains(keyword) ||
                                         c.Description.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(level))
                query = query.Where(c => c.Level == level);

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                "newest" => query.OrderByDescending(c => c.CreatedAt),
                _ => query.OrderBy(c => c.Title)
            };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetByInstructorAsync(int instructorId)
            => await _context.Courses
                .Where(c => c.InstructorId == instructorId)
                .ToListAsync();

        public async Task<int> GetTotalLessonsCountAsync(int courseId)
            => await _context.Lessons
                .CountAsync(l => l.Section.CourseId == courseId);
    }
}