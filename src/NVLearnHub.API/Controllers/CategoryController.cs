using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NVLearnHub.Application.DTOs.Category;
using NVLearnHub.Application.Interfaces.Services;

namespace NVLearnHub.API.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET /api/Category
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll()
        {
            var response = await _categoryService.GetAllAsync();
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        // GET /api/Category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
        {
            var response = await _categoryService.GetByIdAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}