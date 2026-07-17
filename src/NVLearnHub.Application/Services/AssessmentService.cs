using NVLearnHub.Application.DTOs.Assessment;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Domain.Entities.Assessment;

namespace NVLearnHub.Application.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _uow;

        public AssessmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ─── Get Assessment For a Course ─────────────────────────────────────

        public async Task<ApiResponse<AssessmentDto>> GetByCourseAsync(int courseId)
        {
            var assessment = await _uow.Assessments.GetByCourseAsync(courseId);
            if (assessment == null)
                return new ApiResponse<AssessmentDto>(false, "No assessment found for this course.", 404);

            var data = new AssessmentDto
            {
                Id = assessment.Id,
                Title = assessment.Title,
                TimeLimitMinutes = assessment.TimeLimitMinutes,
                PassPercentage = assessment.PassPercentage,
                TotalQuestions = assessment.Questions.Count,
                Questions = assessment.Questions
                    .OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        OrderIndex = q.OrderIndex,
                        Options = q.Options.Select(o => new QuestionOptionDto
                        {
                            Id = o.Id,
                            OptionText = o.OptionText
                            // IsCorrect NOT mapped — never sent to student
                        }).ToList()
                    }).ToList()
            };

            return new ApiResponse<AssessmentDto>(data, "Assessment retrieved successfully.");
        }

        // ─── Start Attempt ────────────────────────────────────────────────────

        public async Task<ApiResponse<StartAttemptResponseDto>> StartAttemptAsync(int assessmentId, int userId)
        {
            var assessment = await _uow.Assessments.GetByIdAsync(assessmentId);
            if (assessment == null)
                return new ApiResponse<StartAttemptResponseDto>(false, "Assessment not found.", 404);

            // Check if student already has an active attempt
            var existingAttempts = await _uow.AssessmentAttempts.GetByAssessmentAsync(assessmentId);
            var activeAttempt = existingAttempts
                .FirstOrDefault(a => a.UserId == userId && a.SubmittedAt == null);

            if (activeAttempt != null)
                return new ApiResponse<StartAttemptResponseDto>(false, "You already have an active attempt.", 400);

            var now = DateTime.UtcNow;

            var attempt = new AssessmentAttempt
            {
                UserId = userId,
                AssessmentId = assessmentId,
                StartedAt = now,
                Score = 0,
                IsPassed = false
            };

            await _uow.AssessmentAttempts.AddAsync(attempt);
            await _uow.SaveChangesAsync();

            var data = new StartAttemptResponseDto
            {
                AttemptId = attempt.Id,
                StartedAt = now,
                ExpiresAt = now.AddMinutes(assessment.TimeLimitMinutes),
                TimeLimitMinutes = assessment.TimeLimitMinutes
            };

            return new ApiResponse<StartAttemptResponseDto>(data, "Attempt started successfully.");
        }

        // ─── Submit Assessment ────────────────────────────────────────────────

        public async Task<ApiResponse<AssessmentResultDto>> SubmitAsync(SubmitAssessmentDto dto, int userId)
        {
            // 1. Get attempt
            var attempt = await _uow.AssessmentAttempts.GetWithAnswersAsync(dto.AttemptId);
            if (attempt == null)
                return new ApiResponse<AssessmentResultDto>(false, "Attempt not found.", 404);

            // 2. Verify attempt belongs to this user
            if (attempt.UserId != userId)
                return new ApiResponse<AssessmentResultDto>(false, "Unauthorized.", 401);

            // 3. Check already submitted
            if (attempt.SubmittedAt != null)
                return new ApiResponse<AssessmentResultDto>(false, "Attempt already submitted.", 400);

            // 4. Get assessment with correct answers
            var assessment = await _uow.Assessments.GetWithQuestionsAndOptionsAsync(attempt.AssessmentId);
            if (assessment == null)
                return new ApiResponse<AssessmentResultDto>(false, "Assessment not found.", 404);

            // 5. Check time limit — auto submit if expired
            var elapsed = DateTime.UtcNow - attempt.StartedAt;
            if (elapsed.TotalMinutes > assessment.TimeLimitMinutes)
                return new ApiResponse<AssessmentResultDto>(false, "Time limit exceeded. Attempt auto-submitted.", 400);

            // 6. Save answers + calculate score
            int correctCount = 0;

            foreach (var answer in dto.Answers)
            {
                var question = assessment.Questions
                    .FirstOrDefault(q => q.Id == answer.QuestionId);

                if (question == null) continue;

                var selectedOption = question.Options
                    .FirstOrDefault(o => o.Id == answer.SelectedOptionId);

                var isCorrect = selectedOption?.IsCorrect ?? false;
                if (isCorrect) correctCount++;

                var assessmentAnswer = new AssessmentAnswer
                {
                    AttemptId = attempt.Id,
                    QuestionId = answer.QuestionId,
                    SelectedOptionId = answer.SelectedOptionId
                };

                await _uow.AssessmentAnswers.AddAsync(assessmentAnswer);
            }

            // 7. Calculate score percentage
            int totalQuestions = assessment.Questions.Count;
            int score = totalQuestions > 0
                ? (int)Math.Round((double)correctCount / totalQuestions * 100)
                : 0;
            bool isPassed = score >= assessment.PassPercentage;

            // 8. Update attempt
            attempt.Score = score;
            attempt.IsPassed = isPassed;
            attempt.SubmittedAt = DateTime.UtcNow;
            _uow.AssessmentAttempts.Update(attempt);

            await _uow.SaveChangesAsync();

            var data = new AssessmentResultDto
            {
                AttemptId = attempt.Id,
                Score = score,
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctCount,
                PassPercentage = assessment.PassPercentage,
                IsPassed = isPassed,
                SubmittedAt = attempt.SubmittedAt.Value
            };

            return new ApiResponse<AssessmentResultDto>(data, isPassed
                ? "Congratulations! You passed the assessment."
                : "You did not pass. Please try again.");
        }

        // ─── Get Attempt Result ───────────────────────────────────────────────

        public async Task<ApiResponse<AssessmentResultDto>> GetAttemptResultAsync(int attemptId, int userId)
        {
            var attempt = await _uow.AssessmentAttempts.GetWithAnswersAsync(attemptId);
            if (attempt == null)
                return new ApiResponse<AssessmentResultDto>(false, "Attempt not found.", 404);

            if (attempt.UserId != userId)
                return new ApiResponse<AssessmentResultDto>(false, "Unauthorized.", 401);

            if (attempt.SubmittedAt == null)
                return new ApiResponse<AssessmentResultDto>(false, "Attempt not yet submitted.", 400);

            var assessment = await _uow.Assessments.GetByIdAsync(attempt.AssessmentId);

            var data = new AssessmentResultDto
            {
                AttemptId = attempt.Id,
                Score = attempt.Score,
                TotalQuestions = assessment?.Questions.Count ?? 0,
                CorrectAnswers = (int)Math.Round(attempt.Score / 100.0 * (assessment?.Questions.Count ?? 0)),
                PassPercentage = assessment?.PassPercentage ?? 0,
                IsPassed = attempt.IsPassed,
                SubmittedAt = attempt.SubmittedAt.Value
            };

            return new ApiResponse<AssessmentResultDto>(data, "Result retrieved successfully.");
        }

        public async Task<ApiResponse<AssessmentStatusDto>> GetStatusAsync(int courseId, int userId)
        {
            // 1. Check if assessment exists for this course
            var assessment = await _uow.Assessments.GetByCourseAsync(courseId);
            if (assessment == null)
                return new ApiResponse<AssessmentStatusDto>(false, "No assessment found for this course.", 404);

            // 2. Get all attempts by this user for this assessment
            var attempts = await _uow.AssessmentAttempts.GetByAssessmentAsync(assessment.Id);
            var userAttempt = attempts
                .Where(a => a.UserId == userId && a.SubmittedAt != null)
                .OrderByDescending(a => a.SubmittedAt)
                .FirstOrDefault();

            // 3. No submitted attempt found
            if (userAttempt == null)
            {
                var notAttempted = new AssessmentStatusDto
                {
                    HasAttempted = false,
                    AttemptId = null,
                    Result = null
                };
                return new ApiResponse<AssessmentStatusDto>(notAttempted, "No completed attempt found.");
            }

            // 4. Found a submitted attempt — return full result
            var result = new AssessmentResultDto
            {
                AttemptId = userAttempt.Id,
                Score = userAttempt.Score,
                TotalQuestions = assessment.Questions.Count,
                CorrectAnswers = (int)Math.Round(userAttempt.Score / 100.0 * assessment.Questions.Count),
                PassPercentage = assessment.PassPercentage,
                IsPassed = userAttempt.IsPassed,
                SubmittedAt = userAttempt.SubmittedAt!.Value
            };

            var data = new AssessmentStatusDto
            {
                HasAttempted = true,
                AttemptId = userAttempt.Id,
                Result = result
            };

            return new ApiResponse<AssessmentStatusDto>(data, "Attempt found.");
        }
    }
}