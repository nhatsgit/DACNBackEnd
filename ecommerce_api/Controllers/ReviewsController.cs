using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Helper;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IAccountRepository _accountRepository;


        public ReviewsController(IOrderRepository _orderRepository, IMapper mapper, IAccountRepository accountRepository, EcomerceDbContext context)
        {
            _accountRepository = accountRepository;
            _context = context;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReviews([FromForm] string? noiDung,
                                            [FromForm] int diem,
                                            [FromForm] int productId,
                                            [FromForm] int orderId,
                                            [FromForm] List<IFormFile>? myFile)
        {

            Review reviews = new Review();
            reviews.NoiDung = noiDung ?? " ";
            reviews.DiemDanhGia = diem;
            reviews.ThoiGianDanhGia = DateTime.Now;
            reviews.ProductId = productId;
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            reviews.CustomerId = user.Id;
            if (ModelState.IsValid)
            {

                var order = await _context.OrderDetails.Where(p => p.OrderId == orderId && p.ProductId == productId).FirstOrDefaultAsync();
                order.IsReview = true;
                _context.Add(reviews);
                await _context.SaveChangesAsync();
                if (myFile != null)
                {
                    foreach (var file in myFile)
                    {
                        if (file.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                var image = new ReviewsImage
                                {
                                    Url = await UploadImage.SaveImage(file),
                                    ReviewsId = reviews.ReviewsId
                                };
                                _context.ReviewsImages.Add(image);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                return Ok(reviews);

            }
            return BadRequest("lỗi");

        }
        [HttpGet("ProductReviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _context.Reviews
                        .Where(r => r.ProductId == productId)
                        .Include(r => r.ReviewsImages)
                        .Include(r=>r.Customer)
                        .Select(r => new 
                        {
                            r.ReviewsId,
                            r.NoiDung,
                            r.DiemDanhGia,
                            r.ThoiGianDanhGia,
                            r.ProductId,
                            Customer = new 
                            {
                                r.Customer.UserName,  
                                r.Customer.Avatar     
                            },
                            ReviewsImages = r.ReviewsImages.Select(ri => new 
                            {
                                ri.Url
                            }).ToList() 
                        })
                        .ToListAsync();
            return Ok(reviews);
        }

    }
}
