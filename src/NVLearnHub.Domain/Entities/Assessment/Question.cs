using NVLearnHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Domain.Entities.Assessment
{
    public class Question : BaseEntity
    {
        public int AssessmentId { get; set; }
        public Assessment Assessment { get; set; } = default!;
        public string QuestionText { get; set; } = default!;
        public int OrderIndex { get; set; }
        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
