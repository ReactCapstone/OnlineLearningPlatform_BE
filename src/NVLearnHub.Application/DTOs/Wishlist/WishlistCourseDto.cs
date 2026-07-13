namespace NVLearnHub.Application.DTOs.Wishlist;

public class WishlistCourseDto
{
    public System.Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructor { get; set; }
    public decimal? Price { get; set; }
}
