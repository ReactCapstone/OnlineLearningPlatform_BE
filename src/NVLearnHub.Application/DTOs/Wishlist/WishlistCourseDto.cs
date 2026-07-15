namespace NVLearnHub.Application.DTOs.Wishlist;

public class WishlistCourseDto
{
    public int Id { get; set; }
    public string Icon { get; set; } = default!;
    public string IconBg { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Duration { get; set; }
    public int? Lessons { get; set; }
}
