using NVLearnHub.Application.DTOs.Assessment;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Domain.Entities.Assessment;
using System.Linq;

namespace NVLearnHub.Application.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _uow;

        public AssessmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ─── Admin: Add questions to an existing assessment ───────────────────

        public async Task<ApiResponse<AssessmentDto>> AddQuestionsAsync(int assessmentId, List<CreateQuestionDto> questions)
        {
            var assessment = await _uow.Assessments.GetByIdAsync(assessmentId);
            if (assessment == null)
                return new ApiResponse<AssessmentDto>(false, "Assessment not found.", 404);

            // Ensure questions collection is loaded/tracked
            var existing = await _uow.Assessments.GetWithQuestionsAndOptionsAsync(assessmentId);
            if (existing == null)
                return new ApiResponse<AssessmentDto>(false, "Assessment not found.", 404);

            if (questions == null || !questions.Any())
                return new ApiResponse<AssessmentDto>(false, "No questions provided.", 400);

            foreach (var q in questions)
            {
                var question = new Question
                {
                    QuestionText = q.QuestionText,
                    OrderIndex = q.OrderIndex,
                    Options = new List<QuestionOption>()
                };

                if (q.Options != null && q.Options.Any())
                {
                    foreach (var opt in q.Options)
                    {
                        question.Options.Add(new QuestionOption
                        {
                            OptionText = opt.OptionText,
                            IsCorrect = opt.IsCorrect
                        });
                    }
                }

                existing.Questions.Add(question);
            }

            _uow.Assessments.Update(existing);
            await _uow.SaveChangesAsync();

            // Map to DTO
            var dto = new AssessmentDto
            {
                Id = existing.Id,
                Title = existing.Title,
                TimeLimitMinutes = existing.TimeLimitMinutes,
                PassPercentage = existing.PassPercentage,
                TotalQuestions = existing.Questions.Count,
                MaxAttempts = existing.MaxAttempts,
                Questions = existing.Questions.OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        OrderIndex = q.OrderIndex,
                        Options = q.Options.Select(o => new QuestionOptionDto
                        {
                            Id = o.Id,
                            OptionText = o.OptionText
                            ,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    }).ToList()
            };

            return new ApiResponse<AssessmentDto>(dto, "Questions added successfully.");
        }

        // ─── Admin: Create Assessment ───────────────────────────────────────

        public async Task<ApiResponse<AssessmentDto>> CreateAssessmentAsync(CreateAssessmentDto dto)
        {
            // ensure course exists
            var course = await _uow.Courses.GetByIdAsync(dto.CourseId);
            if (course == null)
                return new ApiResponse<AssessmentDto>(false, "Course not found.", 404);
            var assessment = new Domain.Entities.Assessment.Assessment
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                TimeLimitMinutes = dto.TimeLimitMinutes,
                PassPercentage = dto.PassPercentage,
                MaxAttempts = dto.MaxAttempts,
                Questions = new List<Question>()
            };

            // Add questions and options if provided
            if (dto.Questions != null && dto.Questions.Any())
            {
                foreach (var q in dto.Questions)
                {
                    var question = new Question
                    {
                        QuestionText = q.QuestionText,
                        OrderIndex = q.OrderIndex,
                        Options = new List<QuestionOption>()
                    };

                    if (q.Options != null && q.Options.Any())
                    {
                        foreach (var opt in q.Options)
                        {
                            question.Options.Add(new QuestionOption
                            {
                                OptionText = opt.OptionText,
                                IsCorrect = opt.IsCorrect
                            });
                        }
                    }

                    assessment.Questions.Add(question);
                }
            }

            await _uow.Assessments.AddAsync(assessment);
            await _uow.SaveChangesAsync();

            var data = new AssessmentDto
            {
                Id = assessment.Id,
                Title = assessment.Title,
                TimeLimitMinutes = assessment.TimeLimitMinutes,
                PassPercentage = assessment.PassPercentage,
                TotalQuestions = assessment.Questions?.Count ?? 0,
                MaxAttempts = assessment.MaxAttempts,
                Questions = assessment.Questions?.OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        OrderIndex = q.OrderIndex,
                        Options = q.Options.Select(o => new QuestionOptionDto
                        {
                            Id = o.Id,
                            OptionText = o.OptionText
                            ,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    }).ToList() ?? new List<QuestionDto>()
            };

            return new ApiResponse<AssessmentDto>(data, "Assessment created successfully.");
        }

        // ─── Admin: Delete Assessment ───────────────────────────────────────

        public async Task<ApiResponse<bool>> DeleteAssessmentAsync(int assessmentId)
        {
            // Load assessment with questions/options to ensure children are tracked
            var assessment = await _uow.Assessments.GetWithQuestionsAndOptionsAsync(assessmentId);
            if (assessment == null)
                return new ApiResponse<bool>(false, "Assessment not found.", 404);

            // Remove any answers for attempts of this assessment, then remove the attempts
            var attempts = await _uow.AssessmentAttempts.GetByAssessmentAsync(assessmentId);
            foreach (var attempt in attempts)
            {
                var answers = await _uow.AssessmentAnswers.FindAsync(a => a.AttemptId == attempt.Id);
                foreach (var ans in answers)
                    _uow.AssessmentAnswers.Remove(ans);

                _uow.AssessmentAttempts.Remove(attempt);
            }

            // At this point, questions and options are tracked as children of assessment (loaded above).
            // Removing the assessment should cascade-delete questions and options if EF Core cascade is configured.
            // Remove the assessment entity
            _uow.Assessments.Remove(assessment);

            await _uow.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Assessment deleted successfully.");
        }

        // ─── Get Assessment(s) For a Course ───────────────────────────────────

        public async Task<ApiResponse<IEnumerable<AssessmentDto>>> GetByCourseAsync(int courseId)
        {
            var assessments = (await _uow.Assessments.GetByCourseAsync(courseId)).ToList();
            if (assessments == null || !assessments.Any())
                return new ApiResponse<IEnumerable<AssessmentDto>>(false, "No assessment found for this course.", 404);

            var data = assessments.Select(assessment => new AssessmentDto
            {
                Id = assessment.Id,
                Title = assessment.Title,
                TimeLimitMinutes = assessment.TimeLimitMinutes,
                PassPercentage = assessment.PassPercentage,
                TotalQuestions = assessment.Questions.Count,
                MaxAttempts = assessment.MaxAttempts,
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
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    }).ToList()
            }).ToList();

            return new ApiResponse<IEnumerable<AssessmentDto>>(data, "Assessments retrieved successfully.");
        }

        // ─── Student: Get assessments (questions/options visible for taking quiz) ──
        public async Task<ApiResponse<IEnumerable<AssessmentDto>>> GetForStudentAsync(int courseId)
        {
            // Return all assessments for the course for student consumption
            var assessments = (await _uow.Assessments.GetByCourseAsync(courseId)).ToList();
            if (assessments == null || !assessments.Any())
                return new ApiResponse<IEnumerable<AssessmentDto>>(false, "No assessment found for this course.", 404);

            var data = assessments.Select(assessment => new AssessmentDto
            {
                Id = assessment.Id,
                Title = assessment.Title,
                TimeLimitMinutes = assessment.TimeLimitMinutes,
                PassPercentage = assessment.PassPercentage,
                TotalQuestions = assessment.Questions.Count,
                MaxAttempts = assessment.MaxAttempts,
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
                            OptionText = o.OptionText,
                            // Do NOT expose correctness to students
                            IsCorrect = null
                        }).ToList()
                    }).ToList()
            }).ToList();

            return new ApiResponse<IEnumerable<AssessmentDto>>(data, "Assessments retrieved successfully.");
        }


        public async Task<ApiResponse<StartAttemptResponseDto>> StartAttemptAsync(int assessmentId, int userId)
        {
            var assessment = await _uow.Assessments.GetByIdAsync(assessmentId);
            if (assessment == null)
                return new ApiResponse<StartAttemptResponseDto>(false, "Assessment not found.", 404);

            var allAttempts = await _uow.AssessmentAttempts.GetByAssessmentAsync(assessmentId);
            var userAttempts = allAttempts.Where(a => a.UserId == userId).ToList();

            // ── Block if genuinely in-progress ───────────────────────────────────
            var activeAttempt = userAttempts.FirstOrDefault(a =>
                a.SubmittedAt == null &&
                a.StartedAt.AddMinutes(assessment.TimeLimitMinutes) > DateTime.UtcNow);

            if (activeAttempt != null)
                return new ApiResponse<StartAttemptResponseDto>(
                    false,
                    "You have an attempt already in progress.",
                    400);

            // ── Check attempt limit (0 = unlimited) ──────────────────────────────
            int submittedCount = userAttempts.Count(a => a.SubmittedAt != null);

            if (assessment.MaxAttempts > 0 && submittedCount >= assessment.MaxAttempts)
            {
                int limit = assessment.MaxAttempts;
                return new ApiResponse<StartAttemptResponseDto>(
                    false,
                    $"You have reached the maximum of {limit} attempt{(limit == 1 ? "" : "s")} for this assessment.",
                    400);
            }

            // ── Create new attempt ────────────────────────────────────────────────
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

            int attemptNumber = submittedCount + 1;

            var data = new StartAttemptResponseDto
            {
                AttemptId = attempt.Id,
                StartedAt = now,
                ExpiresAt = now.AddMinutes(assessment.TimeLimitMinutes),
                TimeLimitMinutes = assessment.TimeLimitMinutes
            };

            return new ApiResponse<StartAttemptResponseDto>(
                data,
                $"Attempt {attemptNumber} started successfully.");
        }

        // ─── Submit Assessment ────────────────────────────────────────────────

        public async Task<ApiResponse<AssessmentResultDto>> SubmitAsync(SubmitAssessmentDto dto, int userId)
        {
            var attempt = await _uow.AssessmentAttempts.GetWithAnswersAsync(dto.AttemptId);
            if (attempt == null)
                return new ApiResponse<AssessmentResultDto>(false, "Attempt not found.", 404);

            if (attempt.UserId != userId)
                return new ApiResponse<AssessmentResultDto>(false, "Unauthorized.", 401);

            if (attempt.SubmittedAt != null)
                return new ApiResponse<AssessmentResultDto>(false, "Attempt already submitted.", 400);

            var assessment = await _uow.Assessments.GetWithQuestionsAndOptionsAsync(attempt.AssessmentId);
            if (assessment == null)
                return new ApiResponse<AssessmentResultDto>(false, "Assessment not found.", 404);

            var elapsed = DateTime.UtcNow - attempt.StartedAt;
            if (elapsed.TotalMinutes > assessment.TimeLimitMinutes)
                return new ApiResponse<AssessmentResultDto>(false, "Time limit exceeded.", 400);

            int correctCount = 0;

            foreach (var answer in dto.Answers)
            {
                var question = assessment.Questions
                    .FirstOrDefault(q => q.Id == answer.QuestionId);

                if (question == null) continue;

                var selectedOption = question.Options
                    .FirstOrDefault(o => o.Id == answer.SelectedOptionId);

                if (selectedOption?.IsCorrect == true) correctCount++;

                await _uow.AssessmentAnswers.AddAsync(new AssessmentAnswer
                {
                    AttemptId = attempt.Id,
                    QuestionId = answer.QuestionId,
                    SelectedOptionId = answer.SelectedOptionId
                });
            }

            int totalQuestions = assessment.Questions.Count;
            int score = totalQuestions > 0
                ? (int)Math.Round((double)correctCount / totalQuestions * 100)
                : 0;
            bool isPassed = score >= assessment.PassPercentage;

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

            return new ApiResponse<AssessmentResultDto>(
                data,
                isPassed
                    ? "Congratulations! You passed."
                    : "You did not pass. You can retake the assessment.");
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

            var assessment = await _uow.Assessments.GetWithQuestionsAndOptionsAsync(attempt.AssessmentId);

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

        // ─── Get Status (Replaces old HasAttempted) ───────────────────────────

        public async Task<ApiResponse<AssessmentStatusDto>> GetStatusAsync(int courseId, int userId)
        {
            var assessments = (await _uow.Assessments.GetByCourseAsync(courseId)).ToList();
            var assessment = assessments.FirstOrDefault();
            if (assessment == null)
                return new ApiResponse<AssessmentStatusDto>(false, "No assessment found for this course.", 404);

            var allAttempts = await _uow.AssessmentAttempts.GetByAssessmentAsync(assessment.Id);
            var userAttempts = allAttempts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartedAt)
                .ToList();

            // In-progress = started, not submitted, not expired
            var activeAttempt = userAttempts.FirstOrDefault(a =>
                a.SubmittedAt == null &&
                a.StartedAt.AddMinutes(assessment.TimeLimitMinutes) > DateTime.UtcNow);

            // Latest submitted
            var latestSubmitted = userAttempts
                .Where(a => a.SubmittedAt != null)
                .OrderByDescending(a => a.SubmittedAt)
                .FirstOrDefault();

            int submittedCount = userAttempts.Count(a => a.SubmittedAt != null);
            bool isUnlimited = assessment.MaxAttempts == 0;
            bool canRetake = isUnlimited || submittedCount < assessment.MaxAttempts;

            AssessmentResultDto? latestResult = null;

            if (latestSubmitted != null)
            {
                latestResult = new AssessmentResultDto
                {
                    AttemptId = latestSubmitted.Id,
                    Score = latestSubmitted.Score,
                    TotalQuestions = assessment.Questions.Count,
                    CorrectAnswers = (int)Math.Round(latestSubmitted.Score / 100.0 * assessment.Questions.Count),
                    PassPercentage = assessment.PassPercentage,
                    IsPassed = latestSubmitted.IsPassed,
                    SubmittedAt = latestSubmitted.SubmittedAt!.Value
                };
            }

            var data = new AssessmentStatusDto
            {
                HasActiveAttempt = activeAttempt != null,
                ActiveAttemptId = activeAttempt?.Id,
                LatestResult = latestResult,
                AttemptCount = submittedCount,
                MaxAttempts = assessment.MaxAttempts,
                AttemptsRemaining = isUnlimited ? null : assessment.MaxAttempts - submittedCount,
                CanRetake = canRetake && activeAttempt == null
            };

            return new ApiResponse<AssessmentStatusDto>(data, "Status retrieved successfully.");
        }


        // ─── Get Full Attempt History ─────────────────────────────────────────

        public async Task<ApiResponse<IEnumerable<AttemptHistoryDto>>> GetAttemptHistoryAsync(int courseId, int userId)
        {
            var assessments = (await _uow.Assessments.GetByCourseAsync(courseId)).ToList();
            var assessment = assessments.FirstOrDefault();
            if (assessment == null)
                return new ApiResponse<IEnumerable<AttemptHistoryDto>>(false, "No assessment found for this course.", 404);

            var allAttempts = await _uow.AssessmentAttempts.GetByAssessmentAsync(assessment.Id);

            var history = allAttempts
                .Where(a => a.UserId == userId && a.SubmittedAt != null)
                .OrderByDescending(a => a.SubmittedAt)
                .Select(a => new AttemptHistoryDto
                {
                    AttemptId = a.Id,
                    Score = a.Score,
                    IsPassed = a.IsPassed,
                    SubmittedAt = a.SubmittedAt!.Value
                });

            return new ApiResponse<IEnumerable<AttemptHistoryDto>>(history, "Attempt history retrieved.");
        }
    }
}