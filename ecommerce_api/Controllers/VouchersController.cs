using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetVouchersCanUse(int shopId)
        {
            var categories = _context.Vouchers
                .Where(v => v.SoLuongCon > 0 && v.NgayHetHan > DateTime.Now && (v.NgayBatDau <= DateTime.Now || v.NgayBatDau == null) && (v.ShopId == shopId || v.ShopId == null))
                .Include(v => v.VoucherCategory);
            return Ok(_mapper.Map<IEnumerable<VoucherDTO>>(categories));
        }

        // GET api/<CategoriesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _context.Vouchers.Where(c => c.VoucherId == id).FirstOrDefaultAsync();
            return Ok(_mapper.Map<VoucherDTO>(category));
        }

        
    }
}
