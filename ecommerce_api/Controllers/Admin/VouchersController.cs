using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ecommerce_api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Developer")]

    public class VouchersController : ControllerBase
    {
        private readonly EcomerceDbContext _context;

        private readonly IMapper _mapper;

        public VouchersController(EcomerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var applicationDbContext = _context.Vouchers.Where(v => v.ShopId == null).Include(v => v.VoucherCategory);
            return Ok(await applicationDbContext.ToListAsync());
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers
                .Include(v => v.VoucherCategory)
                .FirstOrDefaultAsync(m => m.VoucherId == id);
            if (voucher == null || voucher.ShopId != null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }[HttpDelete("{id}")]

        public async Task<IActionResult> EndVoucher(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            var voucher = await _context.Vouchers
                .Include(v => v.VoucherCategory)
                .FirstOrDefaultAsync(m => m.VoucherId == id);

            if (voucher == null || voucher.ShopId !=null)
            {
                return NotFound();
            }
            voucher.NgayHetHan = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(voucher);
        }
        [HttpPost]
        public async Task<IActionResult> Create(VoucherDTO voucherDTO)
        {


            if (voucherDTO.DonToiThieu == null)
            {
                voucherDTO.DonToiThieu = -1;
            }
            if (voucherDTO.GiamToiDa == null)
            {
                voucherDTO.GiamToiDa = -1;
            }
            var voucher = _mapper.Map<Voucher>(voucherDTO);
            voucher.VoucherCategoryId = 1;
            voucher.ShopId = null;
            _context.Add(voucher);
            await _context.SaveChangesAsync();

            return Ok(voucher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditVoucher(int id, [FromBody] VoucherDTO voucherDTO)
        {
            if (voucherDTO == null || id != voucherDTO.VoucherId)
            {
                return BadRequest("Thông tin voucher không hợp lệ.");
            }

            try
            {
                var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userName == null)
                {
                    return Unauthorized();
                }
                var existingVoucher = await _context.Vouchers.FindAsync(id);
                if (existingVoucher == null)
                {
                    return NotFound("Voucher không tồn tại.");
                }


                if (!string.IsNullOrEmpty(voucherDTO.VoucherCode))
                {
                    existingVoucher.VoucherCode = voucherDTO.VoucherCode;
                }
                if (voucherDTO.PhanTramGiam > 0)
                {
                    existingVoucher.PhanTramGiam = voucherDTO.PhanTramGiam;
                }
                if (voucherDTO.NgayHetHan != default)
                {
                    existingVoucher.NgayHetHan = voucherDTO.NgayHetHan;
                }
                if (voucherDTO.SoLuongCon >= 0)
                {
                    existingVoucher.SoLuongCon = voucherDTO.SoLuongCon;
                }
                if (voucherDTO.GiamToiDa.HasValue)
                {
                    existingVoucher.GiamToiDa = voucherDTO.GiamToiDa;
                }
                if (voucherDTO.DonToiThieu.HasValue)
                {
                    existingVoucher.DonToiThieu = voucherDTO.DonToiThieu;
                }
                if (voucherDTO.ShopId.HasValue)
                {
                    existingVoucher.ShopId = null;
                }
                if (voucherDTO.NgayBatDau.HasValue)
                {
                    existingVoucher.NgayBatDau = voucherDTO.NgayBatDau;
                }

                _context.Vouchers.Update(existingVoucher);
                await _context.SaveChangesAsync();

                var resultDTO = _mapper.Map<VoucherDTO>(existingVoucher);
                return Ok(resultDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra khi chỉnh sửa voucher: {ex.Message}");
            }
        }


    }
}
