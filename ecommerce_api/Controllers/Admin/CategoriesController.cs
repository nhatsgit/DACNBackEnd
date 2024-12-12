using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Developer")]

    public class CategoriesController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(EcomerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var category = _mapper.Map<Category>(categoryDTO);
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                var resultDTO = _mapper.Map<CategoryDTO>(category);
                return Ok(resultDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra khi tạo danh mục: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null || id != categoryDTO.ProductCategoryId)
            {
                return BadRequest("Thông tin danh mục không hợp lệ.");
            }

            try
            {
                var existingCategory = await _context.Categories.FindAsync(id);
                if (existingCategory == null)
                {
                    return NotFound("Danh mục không tồn tại.");
                }

                if (!string.IsNullOrEmpty(categoryDTO.TenLoai))
                {
                    existingCategory.TenLoai = categoryDTO.TenLoai;
                }
                if (!string.IsNullOrEmpty(categoryDTO.AnhDaiDien))
                {
                    existingCategory.AnhDaiDien = categoryDTO.AnhDaiDien;
                }

                _context.Categories.Update(existingCategory);
                await _context.SaveChangesAsync();

                var resultDTO = _mapper.Map<CategoryDTO>(existingCategory);
                return Ok(resultDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra khi chỉnh sửa danh mục: {ex.Message}");
            }
        }
    }
}
