namespace NVLearnHub.API.Models
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string Status { get; set; } = default!;
    }
}
