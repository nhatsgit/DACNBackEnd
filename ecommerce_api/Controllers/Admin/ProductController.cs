using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers.Admin
{
    using ecommerce_api.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [Route("api/demo/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "product_read")]
        public IActionResult GetProducts()
        {
            return Ok("Lấy danh sách sản phẩm");
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "product_read")]
        public IActionResult GetProductById(int id)
        {
            return Ok($"Lấy chi tiết sản phẩm {id}");
        }

        [HttpPost]
        [Authorize(Policy = "product_create")]
        public IActionResult CreateProduct()
        {
            return Ok("Tạo sản phẩm thành công");
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "product_update")]
        public IActionResult UpdateProduct(int id)
        {
            return Ok($"Cập nhật sản phẩm {id} thành công");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "product_delete")]
        public IActionResult DeleteProduct(int id)
        {
            return Ok($"Xóa sản phẩm {id} thành công");
        }
    }
}
