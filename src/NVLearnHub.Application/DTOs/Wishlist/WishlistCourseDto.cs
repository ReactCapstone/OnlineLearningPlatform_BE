namespace NVLearnHub.Application.DTOs.Wishlist
{
    public class WishlistCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Thumbnail { get; set; } = default!;
        public string Category { get; set; } = default!;
        public string Level { get; set; } = default!;
        public decimal Price { get; set; }
        public int TotalLessons { get; set; }
        public int TotalSections { get; set; }
    }
}