using NVLearnHub.Application.DTOs.Category;

namespace NVLearnHub.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryDto>>> GetAllAsync();
        Task<ApiResponse<CategoryDto>> GetByIdAsync(int id);
        Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto); // ← add
    }
}