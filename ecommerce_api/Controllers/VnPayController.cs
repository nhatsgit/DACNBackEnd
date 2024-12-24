using AutoMapper;
using ecommerce_api.Models;
using ecommerce_api.Services.VNPAY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVnPayService _vnPayservice;

        public VnPayController(EcomerceDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IVnPayService vnPayservice)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _vnPayservice = vnPayservice;

        }
        [HttpGet("return")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                if (response.VnPayResponseCode == "24")
                {
                    return Ok("Bạn đã hủy thanh toán!");
                }
                else
                {
                    return Ok("Thanh toán ko thành công!");

                }
            }
            var cart = await _context.ShoppingCart
            .Include(c => c.CartItems)
           .ThenInclude(c => c.Product)
           .FirstOrDefaultAsync(c => c.ShoppingCartId == 75);

            if (cart == null)
            {
                return BadRequest("Invalid  data.");

            }
            return Ok("Thành công");
        }
    }
}
