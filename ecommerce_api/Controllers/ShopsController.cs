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
    public class ShopsController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;

        public ShopsController(EcomerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Get()
        {
            var shops = await _context.Shops.ToListAsync();
            return Ok(shops);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var shop = await _context.Shops.Where(c => c.ShopId == id).FirstOrDefaultAsync();

            return Ok(shop);
        }
        [HttpGet("category")]
        public async Task<IActionResult> GetCategory()
        {
            var shopCategory = await _context.ShopCategories.ToListAsync();
            return Ok(shopCategory);
        }
    }
}
