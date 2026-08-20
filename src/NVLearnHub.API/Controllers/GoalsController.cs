using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NVLearnHub.API.Models;
using NVLearnHub.Infrastructure.Data;
using System.Security.Claims;

namespace NVLearnHub.API.Controllers
{
    [Authorize]
    public class GoalsController : BaseController
    {
        private readonly LearnHubDbContext _context;

        public GoalsController(LearnHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<GoalDto>>>> GetForCurrentUser()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized(new ApiResponse<IEnumerable<GoalDto>>(false, "User identity is missing."));

            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId);
            if (user == null)
                return NotFound(new ApiResponse<IEnumerable<GoalDto>>(false, "User not found."));

            var normalizedSkills = user.Expertise
                .Where(skill => !string.IsNullOrWhiteSpace(skill))
                .Select(skill => skill.Trim().ToLower())
                .ToList();

            var matchingSkills = normalizedSkills
                .SelectMany(skill => skill switch
                {
                    "frontend development" => new[] { "frontend development", "react" },
                    "backend development" => new[] { "backend development", ".net" },
                    "full stack development" => new[] { "full stack development", "react", ".net" },
                    "devops & cloud" => new[] { "devops & cloud", "docker" },
                    _ => new[] { skill }
                })
                .ToHashSet();

            var goals = await _context.Goals
                .AsNoTracking()
                .Where(goal => goal.IsActive)
                .Select(goal => new GoalDto
                {
                    Id = goal.Id,
                    Title = goal.Title,
                    Description = goal.Description,
                    RequiredSkill = goal.RequiredSkill
                })
                .ToListAsync();

            goals = goals
                .Where(goal => matchingSkills.Contains(goal.RequiredSkill.Trim().ToLower()))
                .ToList();

            return Ok(new ApiResponse<IEnumerable<GoalDto>>(goals, "Goals loaded successfully."));
        }
    }
}
