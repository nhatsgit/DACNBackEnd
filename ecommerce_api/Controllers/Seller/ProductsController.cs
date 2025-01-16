using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Helper;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using X.PagedList;
using OfficeOpenXml;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

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

            var product = _mapper.Map<Product>(productDto);
            var createdProduct = await _productRepository.AddProduct(product, listImages);

            var imagePath = Path.Combine("wwwroot", createdProduct.AnhDaiDien);

            var result = await ExtractFeatureAndSaveToDbAsync(createdProduct.ProductId, imagePath);

            if (!result)
            {
                return StatusCode(500, "Có lỗi khi lưu vector đặc trưng.");
            }

            return Ok(createdProduct);
        }


        private async Task<bool> ExtractFeatureAndSaveToDbAsync(int productId, string imagePath)
        {
            try
            {
                string pythonScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "AI", "add_features.py");
                string fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", Path.GetFileName(imagePath));
                var startInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{pythonScriptPath}\" {productId} \"{fullImagePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    await process.WaitForExitAsync(); // Chờ script kết thúc

                    if (process.ExitCode == 0)
                    {
                        Console.WriteLine("Feature extraction and save to DB completed successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Error occurred while processing the feature extraction.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling Python script: {ex.Message}");
                return false;
            }
        }
        





        [HttpPost("excelAdd")]
        public async Task<IActionResult> UploadProductFromExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (file == null || file.Length == 0)
            {
                return BadRequest("lỗi file");
            }

            var products = new List<Product>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {

                        var worksheet = package.Workbook.Worksheets.First();
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var product = new Product
                            {
                                ProductId = int.TryParse(worksheet.Cells[row, 999].Text, out int productId) ? productId : 0,
                                TenSp = worksheet.Cells[row, 1].Text,
                                AnhDaiDien = worksheet.Cells[row, 2].Text,
                                MoTa = worksheet.Cells[row, 3].Text,
                                ThongSo = worksheet.Cells[row, 4].Text,
                                GiaNhap = decimal.TryParse(worksheet.Cells[row, 5].Text, out decimal giaNhap) ? giaNhap : 1,
                                GiaBan = decimal.TryParse(worksheet.Cells[row, 6].Text, out decimal giaBan) ? giaBan : 1,
                                SoLuongCon = int.TryParse(worksheet.Cells[row, 7].Text, out int soLuongCon) ? soLuongCon : 1,
                                PhanTramGiam = int.TryParse(worksheet.Cells[row, 8].Text, out int ptg) ? ptg : (int?)null,
                                ProductCategoryId = int.TryParse(worksheet.Cells[row, 9].Text, out int productCategoryId) ? productCategoryId : 1,
                                BrandId = int.TryParse(worksheet.Cells[row, 10].Text, out int brandId) ? brandId : 1,
                                DaAn = false,
                                DiemDanhGia = 0,
                                ShopId = seller.ShopId ?? 0,
                            };
                            products.Add(product);
                        }
                    }
                }

                var productsadd = _productRepository.AddRangeProduct(products);
            }
            catch (Exception e)
            {

                return BadRequest("dữ liệu file sai"+e.Message);
            }
            return Ok("thêm thành công");
        }


    }
    
}
