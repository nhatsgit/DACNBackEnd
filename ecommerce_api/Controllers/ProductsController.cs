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
using X.PagedList;
using AutoMapper;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]

        public async Task<ActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProductById(id);
                if(product.DaAn==true)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpGet("p{id}")]

        public async Task<ActionResult> GetProductImage(int id)
        {
            try
            {
                var product = await _productRepository.GetProductById(id);
                if(product.DaAn==true)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // PUT: api/Products/5
        [HttpGet("productImage/{id}")]
        public async Task<ActionResult> GetProductImages(int id)
        {
            try
            {
                var productImages = await _productRepository.GetProductImagesById(id);
                
                return Ok(productImages);
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
            var product = _mapper.Map<Product>(productDto);
            var createdProduct = await _productRepository.AddProduct(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.DeleteProduct(id);

                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpGet("query")]
        public async Task<IActionResult> QueryProducts(
            [FromQuery] string? keyword, [FromQuery] int? categoryId,
            [FromQuery] int? brandId,[FromQuery] int? shopId, 
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, 
            [FromQuery] int pageNumber=1 , [FromQuery] int pageSize = 10)
        {
            var products = await _productRepository.QueryProducts( keyword, categoryId,brandId,shopId, minPrice, maxPrice) ;
            
            if (products == null || !products.Any())
            {
                return NotFound("No products found matching your search criteria.");
            }
            return Ok(new PagedListDTO<ProductDTO>(_mapper.Map<IEnumerable<ProductDTO>>(products).ToPagedList(pageNumber,pageSize)));
        }
        [HttpGet("searchSuggestions")]
        public async Task<List<string>> SearchSuggestions([FromQuery] string keyword)
        {
            return await _productRepository.GetSearchSuggestions(keyword) ;
        }
        [HttpGet("suggestionsToday")]
        public async Task<IActionResult> Get12RandomProducts()
        {
            var randomProducts = await _productRepository.GetRandomProducts(12);

            if (randomProducts == null || !randomProducts.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(_mapper.Map<IEnumerable<ProductDTO>>(randomProducts));
        }
        [HttpGet("sliderBar")]
        public async Task<IActionResult> Get3RandomProducts()
        {
            var randomProducts = await _productRepository.GetRandomProducts(3);

            if (randomProducts == null || !randomProducts.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(_mapper.Map<IEnumerable<ProductDTO>>(randomProducts));
        }
        
    }
}
