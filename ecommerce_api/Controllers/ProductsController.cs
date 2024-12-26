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
using ecommerce_api.Migrations;
using ecommerce_api.Controllers.Seller;
using ecommerce_api.Helper;
using System.Diagnostics;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                if(product.DaAn==true||product.Shop.BiChan==true)
                {
                    return BadRequest();
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

        [HttpGet("query")]
        public async Task<IActionResult> QueryProducts(
            [FromQuery] string? keyword, [FromQuery] int? categoryId,
            [FromQuery] int? brandId,[FromQuery] int? shopId, 
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
            [FromQuery] bool? daAn = false, [FromQuery] bool? daHet=false,
            [FromQuery] int pageNumber=1 , [FromQuery] int pageSize = 10)
        {
            var products = await _productRepository.QueryProducts( keyword, categoryId,brandId,shopId, minPrice, maxPrice, daAn, daHet) ;
            var result=products.Where(p=>p.Shop.BiChan!=true).ToList();
            if (result == null || !result.Any())
            {
                return NotFound("No products found matching your search criteria.");
            }
            return Ok(new PagedListDTO<ProductDTO>(_mapper.Map<IEnumerable<ProductDTO>>(result).ToPagedList(pageNumber,pageSize)));
        }
        [HttpGet("getCategoriesFromQuerry")]
        public async Task<IActionResult> GetCategories(
            [FromQuery] string? keyword,
            [FromQuery] int? brandId,[FromQuery] int? shopId, 
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
            [FromQuery] bool? daAn=false, [FromQuery] bool? daHet=false)
        {
            var categories = await _productRepository.GetCategoryFromQuerry(keyword,brandId,shopId, minPrice, maxPrice, daAn, daHet) ;
            
            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found matching your search criteria.");
            }
            var result=categories.Select(c => new
            {
                id = c.ProductCategoryId,
                tenLoai = c.TenLoai
            })
            .ToList();
            return Ok(result);
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
        [AllowAnonymous]
        [HttpPost("searchByImage")]
        public async Task<ActionResult<List<ProductSimilarity>>> UploadAndCompareImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            try
            {
                // Lưu ảnh vào thư mục gốc
                var filePath = await UploadImage.SaveImage(image);

                var result = await CompareFeaturesWithPythonScript(filePath);
                if (result == null || !result.Any())
                {
                    return NotFound("No products found matching your search criteria.");
                }
                if (result == null || result.Count == 0)
                {
                    return NotFound("No similar products found.");
                }
                var productIds = result.Select(r => r.ProductId).ToList();
                var products = await _productRepository.GetProductsByIds(productIds);


                if (products == null || !products.Any())
                {
                    return NotFound("No products found matching your search criteria.");
                }
                return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
        private async Task<List<ProductSimilarity>> CompareFeaturesWithPythonScript(string imagePath)
        {
            try
            {
                string pythonScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "AI", "compare_features.py");
                string fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", Path.GetFileName(imagePath));
                var startInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{pythonScriptPath}\" \"{fullImagePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        var output = await reader.ReadToEndAsync();
                        var productSimilarities = ParseSimilarityResults(output);
                        return productSimilarities;
                    }

                    await process.WaitForExitAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while calling Python script: {ex.Message}");
                return null;
            }
        }

        private List<ProductSimilarity> ParseSimilarityResults(string output)
        {
            var similarities = new List<ProductSimilarity>();

            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0].Split(':')[1].Trim(), out int productId) &&
                        float.TryParse(parts[1].Split(':')[1].Trim(), out float similarity))
                    {
                        similarities.Add(new ProductSimilarity
                        {
                            ProductId = productId,
                            Similarity = similarity
                        });
                    }
                }
            }

            return similarities.OrderByDescending(s => s.Similarity).ToList();
        }

    }
    public class ProductSimilarity
    {
        public int ProductId { get; set; }
        public float Similarity { get; set; }
    }
}
