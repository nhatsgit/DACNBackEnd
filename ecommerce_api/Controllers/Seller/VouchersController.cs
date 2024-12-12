using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Drawing.Printing;
using System.Security.Claims;
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;

namespace ecommerce_api.Controllers.Seller
{
    [Route("api/seller/[controller]")]
    [ApiController]
    [Authorize(Roles = "Developer,ShopOwner,ShopStaff")]

    public class VouchersController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;


        public VouchersController(EcomerceDbContext context, IMapper mapper,IAccountRepository accountRepository)
        {
            _context = context;
            _mapper = mapper;
            _accountRepository = accountRepository;

        }
        [HttpGet]
        public async Task<IActionResult> GetVouchers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var vouchers =await _context.Vouchers.Where(v => v.ShopId == seller.ShopId).Include(v => v.VoucherCategory).ToListAsync();

            return Ok(new PagedListDTO<Voucher>(_mapper.Map<IEnumerable<Voucher>>(vouchers).ToPagedList(pageNumber, pageSize)));


        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var voucher = await _context.Vouchers
                .Include(v => v.VoucherCategory)
                .FirstOrDefaultAsync(m => m.VoucherId == id);
            if (voucher == null || voucher.ShopId != seller.ShopId)
            {
                return NotFound();
            }

            return Ok(voucher);
        }
        [HttpGet("Expired")]
        public async Task<IActionResult> VoucherExpired([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var vouchers = await _context.Vouchers.Where(v => v.ShopId == seller.ShopId && v.NgayHetHan < DateTime.Now).Include(v => v.VoucherCategory).ToListAsync();



            return Ok(new PagedListDTO<Voucher>(_mapper.Map<IEnumerable<Voucher>>(vouchers).ToPagedList(pageNumber, pageSize)));

        }
        [HttpGet("Unamount")]
        public async Task<IActionResult> VoucherUnamount([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var vouchers =await _context.Vouchers.Where(v => v.ShopId == seller.ShopId && v.SoLuongCon <= 0).Include(v => v.VoucherCategory).ToListAsync();



            return Ok(new PagedListDTO<Voucher>(_mapper.Map<IEnumerable<Voucher>>(vouchers).ToPagedList(pageNumber, pageSize)));

        }
        [HttpPost]
        public async Task<IActionResult> Create(VoucherDTO voucherDTO)
        {
 
                var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userName == null)
                {
                    return Unauthorized();
                }
                var seller = await _accountRepository.GetCurrentUserAsync(userName);
                if (voucherDTO.DonToiThieu == null)
                {
                voucherDTO.DonToiThieu = -1;
                }
                if (voucherDTO.GiamToiDa == null)
                {
                voucherDTO.GiamToiDa = -1;
                }
                var voucher=_mapper.Map<Voucher>(voucherDTO);
            voucher.VoucherCategoryId = 1;
                voucher.ShopId = seller.ShopId ?? 0;
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
                var seller = await _accountRepository.GetCurrentUserAsync(userName);
                var existingVoucher= _mapper.Map<Voucher>(voucherDTO);
                existingVoucher.ShopId=seller.ShopId;
                existingVoucher.VoucherCategoryId=1;
                if (existingVoucher.GiamToiDa == null)
                {
                    existingVoucher.GiamToiDa = -1;
                }if (existingVoucher.DonToiThieu == null)
                {
                    existingVoucher.DonToiThieu = -1;
                }

                _context.Vouchers.Update(existingVoucher);
                await _context.SaveChangesAsync();

                return Ok(existingVoucher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra khi chỉnh sửa voucher: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> EndVoucher(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var voucher = await _context.Vouchers
                .Include(v => v.VoucherCategory)
                .FirstOrDefaultAsync(m => m.VoucherId == id);
            
            if (voucher == null || voucher.ShopId != seller.ShopId)
            {
                return NotFound();
            }
            voucher.NgayHetHan=DateTime.Now;
                await _context.SaveChangesAsync();

            return Ok(voucher);
        }

    }
}
