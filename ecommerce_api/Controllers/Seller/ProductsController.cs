using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Helper;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList;

namespace ecommerce_api.Controllers.Seller
{
    [Route("api/seller/[controller]")]
    [ApiController]
    [Authorize(Roles = "Developer,ShopOwner,ShopStaff")]

    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;


        public ProductsController(IMapper mapper, IProductRepository productRepository,IAccountRepository accountRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _accountRepository = accountRepository;
        }


        

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            try
            {
                var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userName == null)
                {
                    return Unauthorized();
                }
                var user = await _accountRepository.GetCurrentUserAsync(userName);
                var product = await _productRepository.GetProductById(id);
                if (product.ShopId!=user.ShopId)
                {
                    return BadRequest();
                }
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }




        
        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> _SwapHideShowProduct(int id)
        {
            try
            {
                var product = await _productRepository.SwapHideShowProduct(id);

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
            [FromQuery] int? brandId, 
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
            [FromQuery] bool? daAn = null, [FromQuery] bool? daHet = null,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var products = await _productRepository.QueryProducts(keyword, categoryId, brandId, user.ShopId, minPrice, maxPrice, daAn, daHet);

            if (products == null || !products.Any())
            {
                return NotFound("No products found matching your search criteria.");
            }
            return Ok(new PagedListDTO<ProductDTO>(_mapper.Map<IEnumerable<ProductDTO>>(products).ToPagedList(pageNumber, pageSize)));
        }
        [HttpGet("getCategoriesFromQuerry")]
        public async Task<IActionResult> GetCategories(
            [FromQuery] string? keyword,
            [FromQuery] int? brandId, [FromQuery] int? shopId,
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var categories = await _productRepository.GetCategoryFromQuerry(keyword, brandId, shopId, minPrice, maxPrice, null, null);

            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found matching your search criteria.");
            }
            var result = categories.Select(c => new
            {
                id = c.ProductCategoryId,
                tenLoai = c.TenLoai
            })
            .ToList();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDTO product, List<IFormFile> listImages, IFormFile? anhDaiDien = null)
        {
            if (product == null || id != product.ProductId)
            {
                return BadRequest("Thông tin sản phẩm không hợp lệ.");
            }
            if (anhDaiDien != null)
            {
                product.AnhDaiDien = await UploadImage.SaveImage(anhDaiDien);
            }
            var productMap = _mapper.Map<Product>(product);
            var updatedProduct = await _productRepository.UpdateProduct(id, productMap, listImages);

            if (updatedProduct == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            return Ok("Cập nhật sản phẩm thành công");
        }
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] ProductDTO productDto, IFormFile anhDaiDien, List<IFormFile> listImages)
        {
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            if (user?.ShopId == null)
            {
                return BadRequest("Người dùng không thuộc shop nào.");
            }

            productDto.ShopId = user.ShopId.Value;

            if (anhDaiDien != null)
            {
                productDto.AnhDaiDien = await UploadImage.SaveImage(anhDaiDien);
            }

            // Map DTO sang Entity và lưu sản phẩm
            var product = _mapper.Map<Product>(productDto);
            var createdProduct = await _productRepository.AddProduct(product, listImages);

            return Ok(createdProduct);
        }


    }
}
