using AutoMapper;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        public ReviewsController(EcomerceDbContext context)
        {
            _context = context;
        }
        // GET: api/<ReviewsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ReviewsController>/5
        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int productId)
        {
            var reviews=await _context.Reviews.Where(r=>r.ProductId==productId).ToListAsync();
            return Ok(reviews);
        }

        // POST api/<ReviewsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {

            return Ok();
        }

        // PUT api/<ReviewsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ReviewsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
