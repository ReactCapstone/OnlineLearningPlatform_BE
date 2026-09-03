using NVLearnHub.Application.DTOs.Category;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Domain.Entities.Catalog;

namespace NVLearnHub.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _uow.Categories.GetAllWithSubCategoriesAsync();

            var data = categories.Select(MapToDto).ToList();

            return new ApiResponse<List<CategoryDto>>(data, "Categories retrieved successfully.");
        }

        public async Task<ApiResponse<CategoryDto>> GetByIdAsync(int id)
        {
            var category = await _uow.Categories.GetWithSubCategoriesAsync(id);
            if (category == null)
                return new ApiResponse<CategoryDto>(false, "Category not found.", 404);

            return new ApiResponse<CategoryDto>(MapToDto(category), "Category retrieved successfully.");
        }
        public async Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto)
        {
            // Check duplicate name
            var existing = await _uow.Categories.FindAsync(c => c.Name == dto.Name);
            if (existing.Any())
                return new ApiResponse<CategoryDto>(false, "Category with this name already exists.", 409);

            var category = new Category
            {
                Name = dto.Name,
                ParentCategoryId = null  // hardcoded null for now
            };

            await _uow.Categories.AddAsync(category);
            await _uow.SaveChangesAsync();

            var data = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = null,
                SubCategories = new()
            };

            return new ApiResponse<CategoryDto>(data, "Category created successfully.");
        }

        private static CategoryDto MapToDto(NVLearnHub.Domain.Entities.Catalog.Category c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            ParentCategoryId = c.ParentCategoryId,
            SubCategories = c.SubCategories?
                .Select(MapToDto)
                .ToList() ?? new()
        };
    }
}