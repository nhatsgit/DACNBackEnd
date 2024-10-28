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
    public class BrandsController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;

        public BrandsController(EcomerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
     
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var brands = await _context.Brands.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<BrandDTO>>(brands));
        }

   
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var brand = await _context.Brands.Where(c => c.BrandId == id).FirstOrDefaultAsync();
            return Ok(_mapper.Map<BrandDTO>(brand));
        }

        // POST api/<BrandsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BrandsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BrandsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
