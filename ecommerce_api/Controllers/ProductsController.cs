using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using ecommerce_api.Repostitories;
using ecommerce_api.DTO;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;


        public ProductsController(EcomerceDbContext context, IProductRepository productRepository)
        {

            _productRepository = productRepository;

        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]

        public async Task<ActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProductById(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product=await _productRepository.UpdateProduct(id, productDto);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProduct = await _productRepository.AddProduct(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.DeleteProduct(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


    }
}
