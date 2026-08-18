using NVLearnHub.Application.DTOs.Category;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;

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