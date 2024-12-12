using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Developer")]

    public class BrandsController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;

        public BrandsController(EcomerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> AddBrand([FromBody] BrandDTO brandDTO)
        {
            if (brandDTO == null || string.IsNullOrEmpty(brandDTO.TenNhanHieu))
            {
                return BadRequest("Thông tin thương hiệu không hợp lệ.");
            }

            try
            {
                var brand = _mapper.Map<Brand>(brandDTO);

                await _context.Brands.AddAsync(brand);
                await _context.SaveChangesAsync();

                var resultDTO = _mapper.Map<BrandDTO>(brand);
                return CreatedAtAction(nameof(AddBrand), new { id = brand.BrandId }, resultDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra khi thêm thương hiệu: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditBrand(int id, [FromBody] BrandDTO brandDTO)
        {
            if (brandDTO == null || id != brandDTO.BrandId)
            {
                return BadRequest("Thông tin thương hiệu không hợp lệ.");
            }

            try
            {
                var existingBrand = await _context.Brands.FindAsync(id);
                if (existingBrand == null)
                {
                    return NotFound("Thương hiệu không tồn tại.");
                }

                if (!string.IsNullOrEmpty(brandDTO.TenNhanHieu))
                {
                    existingBrand.TenNhanHieu = brandDTO.TenNhanHieu;
                }

                _context.Brands.Update(existingBrand);
                await _context.SaveChangesAsync();

                var resultDTO = _mapper.Map<BrandDTO>(existingBrand);
                return Ok(resultDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra khi chỉnh sửa thương hiệu: {ex.Message}");
            }
        }

    }
}
