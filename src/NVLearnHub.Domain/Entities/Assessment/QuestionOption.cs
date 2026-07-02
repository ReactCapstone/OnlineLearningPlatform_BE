using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Assessment
{
    public class QuestionOption : BaseEntity
    {
        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;
        public string OptionText { get; set; } = default!;
        public bool IsCorrect { get; set; }
    }
}
