using NVLearnHub.Application.DTOs.Dashboard;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;

namespace NVLearnHub.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;

        public DashboardService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ─── Stats ────────────────────────────────────────────────────────────

        public async Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(int userId)
        {
            var enrollments = await _uow.Enrollments.GetByUserAsync(userId);
            var certificates = await _uow.Certificates.GetByUserAsync(userId);

            var enrollmentList = enrollments.ToList();

            var data = new DashboardStatsDto
            {
                EnrolledCourses = enrollmentList.Count,
                InProgressCourses = enrollmentList.Count(e => e.Status == "InProgress"),
                CompletedCourses = enrollmentList.Count(e => e.Status == "Completed"),
                CertificatesEarned = certificates.Count()
            };

            return new ApiResponse<DashboardStatsDto>(data, "Stats retrieved successfully.");
        }

        // ─── Continue Learning ────────────────────────────────────────────────

        public async Task<ApiResponse<List<ContinueLearningDto>>> GetContinueLearningAsync(int userId)
        {
            var enrollments = await _uow.Enrollments.GetByUserAsync(userId);

            var inProgress = enrollments
                .Where(e => e.Status == "InProgress")
                .Take(3)
                .ToList();

            var result = new List<ContinueLearningDto>();

            foreach (var enrollment in inProgress)
            {
                // Total lessons for this course
                var totalLessons = enrollment.Course.Sections
                    .SelectMany(s => s.Lessons)
                    .Count();

                // Completed lessons
                var completedProgress = await _uow.LessonProgress
                    .GetByEnrollmentAsync(enrollment.Id);

                var completedLessons = completedProgress
                    .Count(lp => lp.IsCompleted);

                // Current lesson (first incomplete)
                var allLessons = enrollment.Course.Sections
                    .OrderBy(s => s.OrderIndex)
                    .SelectMany(s => s.Lessons.OrderBy(l => l.OrderIndex))
                    .ToList();

                var completedLessonIds = completedProgress
                    .Where(lp => lp.IsCompleted)
                    .Select(lp => lp.LessonId)
                    .ToHashSet();

                var currentLesson = allLessons
                    .FirstOrDefault(l => !completedLessonIds.Contains(l.Id));

                int progress = totalLessons > 0
                    ? (int)Math.Round((double)completedLessons / totalLessons * 100)
                    : 0;

                result.Add(new ContinueLearningDto
                {
                    CourseId = enrollment.Course.Id,
                    Title = enrollment.Course.Title,
                    Instructor = enrollment.Course.Instructor?.FullName ?? string.Empty,
                    Thumbnail = enrollment.Course.Thumbnail,
                    TotalLessons = totalLessons,
                    CompletedLessons = completedLessons,
                    ProgressPercent = progress,
                    CurrentLesson = currentLesson?.Title
                });
            }

            return new ApiResponse<List<ContinueLearningDto>>(result, "Continue learning retrieved.");
        }

        // ─── Recommended Courses ──────────────────────────────────────────────

        public async Task<ApiResponse<List<RecommendedCourseDto>>> GetRecommendedCoursesAsync(int userId)
        {
            // Get courses the user is already enrolled in
            var enrollments = await _uow.Enrollments.GetByUserAsync(userId);
            var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToHashSet();

            // Get all published courses not enrolled in
            var allCourses = await _uow.Courses.SearchAsync(null, null, null, "newest");

            var recommended = allCourses
                .Where(c => !enrolledCourseIds.Contains(c.Id))
                .Take(4)
                .Select(c => new RecommendedCourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Instructor = c.Instructor?.FullName ?? string.Empty,
                    Thumbnail = c.Thumbnail,
                    Category = c.Category?.Name ?? string.Empty,
                    Level = c.Level,
                    Price = c.Price,
                    TotalLessons = c.Sections.SelectMany(s => s.Lessons).Count()
                })
                .ToList();

            return new ApiResponse<List<RecommendedCourseDto>>(recommended, "Recommended courses retrieved.");
        }

        // ─── Recent Activity ──────────────────────────────────────────────────

        public async Task<ApiResponse<List<RecentActivityDto>>> GetRecentActivityAsync(int userId)
        {
            var activities = new List<RecentActivityDto>();

            // Lesson completions
            var enrollments = await _uow.Enrollments.GetByUserAsync(userId);
            foreach (var enrollment in enrollments)
            {
                var progress = await _uow.LessonProgress.GetByEnrollmentAsync(enrollment.Id);
                foreach (var lp in progress.Where(p => p.IsCompleted && p.CompletedAt != null))
                {
                    activities.Add(new RecentActivityDto
                    {
                        ActivityType = "LessonCompleted",
                        Title = $"Completed: {lp.Lesson?.Title ?? "a lesson"}",
                        ActivityDate = lp.CompletedAt!.Value,
                        TimeAgo = GetTimeAgo(lp.CompletedAt.Value)
                    });
                }
            }

            // Quiz submissions
            var attempts = await _uow.AssessmentAttempts.GetByUserAsync(userId);
            foreach (var attempt in attempts.Where(a => a.SubmittedAt != null))
            {
                activities.Add(new RecentActivityDto
                {
                    ActivityType = "QuizSubmitted",
                    Title = $"Quiz Submitted — Score: {attempt.Score}%",
                    ActivityDate = attempt.SubmittedAt!.Value,
                    TimeAgo = GetTimeAgo(attempt.SubmittedAt.Value)
                });
            }

            // Certificates
            var certificates = await _uow.Certificates.GetByUserAsync(userId);
            foreach (var cert in certificates)
            {
                activities.Add(new RecentActivityDto
                {
                    ActivityType = "CertificateEarned",
                    Title = "Certificate Earned",
                    ActivityDate = cert.CreatedAt,
                    TimeAgo = GetTimeAgo(cert.CreatedAt)
                });
            }

            var result = activities
                .OrderByDescending(a => a.ActivityDate)
                .Take(5)
                .ToList();

            return new ApiResponse<List<RecentActivityDto>>(result, "Recent activity retrieved.");
        }
        // ─── Upcoming Classes ─────────────────────────────────────────────────────

        public async Task<ApiResponse<List<UpcomingClassDto>>> GetUpcomingClassesAsync(int userId)
        {
            // Get enrolled courses
            var enrollments = await _uow.Enrollments.GetByUserAsync(userId);
            var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToHashSet();

            // Get lessons from enrolled courses that haven't been completed yet
            // and are scheduled in the future — for now we use lesson order
            // as a proxy for "upcoming" since we don't have a schedule table yet
            var result = new List<UpcomingClassDto>();

            foreach (var enrollment in enrollments.Where(e => e.Status == "InProgress").Take(5))
            {
                var progress = await _uow.LessonProgress.GetByEnrollmentAsync(enrollment.Id);
                var completedIds = progress
                    .Where(lp => lp.IsCompleted)
                    .Select(lp => lp.LessonId)
                    .ToHashSet();

                var nextLesson = enrollment.Course.Sections
                    .OrderBy(s => s.OrderIndex)
                    .SelectMany(s => s.Lessons.OrderBy(l => l.OrderIndex))
                    .FirstOrDefault(l => !completedIds.Contains(l.Id));

                if (nextLesson == null) continue;

                // Use a calculated upcoming time based on day offset per enrollment
                var scheduledAt = DateTime.UtcNow.Date
                    .AddDays(result.Count + 1)
                    .AddHours(19); // default 7PM

                result.Add(new UpcomingClassDto
                {
                    Id = nextLesson.Id,
                    Title = nextLesson.Title,
                    Instructor = enrollment.Course.Instructor?.FullName ?? string.Empty,
                    DayOfWeek = scheduledAt.DayOfWeek.ToString(),
                    Time = scheduledAt.ToString("h:mm tt"),
                    ScheduledAt = scheduledAt
                });
            }

            return new ApiResponse<List<UpcomingClassDto>>(result, "Upcoming classes retrieved.");
        }

        // ─── Weekly Goal ──────────────────────────────────────────────────────

        public async Task<ApiResponse<WeeklyGoalDto>> GetWeeklyGoalAsync(int userId)
        {
            const int weeklyGoal = 5; // lessons per week — make this configurable later

            var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var weekEnd = weekStart.AddDays(7);

            var enrollments = await _uow.Enrollments.GetByUserAsync(userId);

            int completedThisWeek = 0;

            foreach (var enrollment in enrollments)
            {
                var progress = await _uow.LessonProgress.GetByEnrollmentAsync(enrollment.Id);
                completedThisWeek += progress.Count(lp =>
                    lp.IsCompleted &&
                    lp.CompletedAt.HasValue &&
                    lp.CompletedAt >= weekStart &&
                    lp.CompletedAt < weekEnd);
            }

            int percent = weeklyGoal > 0
                ? Math.Min((int)Math.Round((double)completedThisWeek / weeklyGoal * 100), 100)
                : 0;

            var data = new WeeklyGoalDto
            {
                GoalLessons = weeklyGoal,
                CompletedThisWeek = completedThisWeek,
                ProgressPercent = percent
            };

            return new ApiResponse<WeeklyGoalDto>(data, "Weekly goal retrieved.");
        }

        // ─── Helper ───────────────────────────────────────────────────────────

        private static string GetTimeAgo(DateTime date)
        {
            var diff = DateTime.UtcNow - date;

            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes} minutes ago";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours} hours ago";
            if (diff.TotalDays < 2)
                return "Yesterday";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} days ago";

            return date.ToString("MMM dd, yyyy");
        }
    }
}