using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Assessment
{
    public class AssessmentAnswer : BaseEntity
    {
        public int AttemptId { get; set; }
        public AssessmentAttempt Attempt { get; set; } = default!;
        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;
        public int SelectedOptionId { get; set; }
        public QuestionOption SelectedOption { get; set; } = default!;
    }
}
