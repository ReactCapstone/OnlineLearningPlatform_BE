using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Application.DTOs.Assessment
{
    public class UpdateAssessmentDto
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = default!;

        public int TimeLimitMinutes { get; set; }

        public int PassPercentage { get; set; }

        public int MaxAttempts { get; set; }

        public List<UpdateQuestionDto> Questions { get; set; } = new();
    }

    public class UpdateQuestionDto
    {
        public string QuestionText { get; set; } = default!;

        public int OrderIndex { get; set; }

        public List<UpdateQuestionOptionDto> Options { get; set; } = new();
    }

    public class UpdateQuestionOptionDto
    {
        public string OptionText { get; set; } = default!;

        public bool IsCorrect { get; set; }
    }
}
