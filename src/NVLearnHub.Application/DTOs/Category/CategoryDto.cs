namespace NVLearnHub.Application.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int? ParentCategoryId { get; set; }
        public List<CategoryDto> SubCategories { get; set; } = new();
    }
}