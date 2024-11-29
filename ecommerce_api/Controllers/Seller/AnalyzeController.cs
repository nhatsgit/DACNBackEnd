using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace ecommerce_api.Controllers.Seller
{
    [Route("api/seller/[controller]")]
    [ApiController]
    [Authorize(Roles = "Developer,ShopOwner")]

    public class AnalyzeController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;


        public AnalyzeController(EcomerceDbContext context, IMapper mapper, IAccountRepository accountRepository)
        {
            _context = context;
            _mapper = mapper;
            _accountRepository = accountRepository;

        }
        [HttpGet("OrderAnalyze")]
        public async Task<IActionResult> OrderAnalyze(int mode)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var ordersshop = await _context.Orders
                .Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == seller.ShopId)
                .ToListAsync();
            if (mode == 999)
            {
                var tongDon = ordersshop
                .Count();
                var DonChoXacNhan = ordersshop
                    .Count(o => o.OrderStatusId == 1);
                var DonDaThanhToan = ordersshop
                    .Count(o => o.OrderStatusId == 5);
                var result = new OrderAnalyzeDTO() 
                {TongDon=tongDon,
                    DonChoXacNhan=DonChoXacNhan,
                    DonDaThanhToan=DonDaThanhToan };
                return Ok(result);
            }else if (mode == 0)
            {
                var TongDon = ordersshop
                .Count(o => o.OrderDate.Date == DateTime.Today);
                var DonChoXacNhan = ordersshop
                    .Count(o => o.OrderStatusId == 1 && o.OrderDate.Date == DateTime.Today);
                var DonDaThanhToan = ordersshop
                    .Count(o => o.OrderStatusId == 5 && o.OrderDate.Date == DateTime.Today);
                var result = new OrderAnalyzeDTO()
                {
                    TongDon = TongDon,
                    DonChoXacNhan = DonChoXacNhan,
                    DonDaThanhToan = DonDaThanhToan
                };
                return Ok(result);
            } else if (mode == 1)
            {
                var TongDon = ordersshop
                .Count(o => o.OrderDate.Date == DateTime.Today.AddDays(-1));
                var DonChoXacNhan = ordersshop
                    .Count(o => o.OrderStatusId == 1 && o.OrderDate.Date == DateTime.Today.AddDays(-1));
                var DonDaThanhToan = ordersshop
                    .Count(o => o.OrderStatusId == 5 && o.OrderDate.Date == DateTime.Today.AddDays(-1));
                var result = new OrderAnalyzeDTO()
                {
                    TongDon = TongDon,
                    DonChoXacNhan = DonChoXacNhan,
                    DonDaThanhToan = DonDaThanhToan
                };
                return Ok(result);
            } else if (mode == 7)
            {
                var TongDon = ordersshop
                .Count(o => o.OrderDate.Date == DateTime.Today.AddDays(-7));
                var DonChoXacNhan = ordersshop
                    .Count(o => o.OrderStatusId == 1 && o.OrderDate.Date == DateTime.Today.AddDays(-7));
                var DonDaThanhToan = ordersshop
                    .Count(o => o.OrderStatusId == 5 && o.OrderDate.Date == DateTime.Today.AddDays(-7));
                var result = new OrderAnalyzeDTO()
                {
                    TongDon = TongDon,
                    DonChoXacNhan = DonChoXacNhan,
                    DonDaThanhToan = DonDaThanhToan
                };
                return Ok(result);
            }
            else if (mode == 30)
            {
                var TongDon = ordersshop
                .Count(o => o.OrderDate.Date == DateTime.Today.AddDays(-30));
                var DonChoXacNhan = ordersshop
                    .Count(o => o.OrderStatusId == 1 && o.OrderDate.Date == DateTime.Today.AddDays(-30));
                var DonDaThanhToan = ordersshop
                    .Count(o => o.OrderStatusId == 5 && o.OrderDate.Date == DateTime.Today.AddDays(-30));
                var result = new OrderAnalyzeDTO()
                {
                    TongDon = TongDon,
                    DonChoXacNhan = DonChoXacNhan,
                    DonDaThanhToan = DonDaThanhToan
                };
                return Ok(result);
            }


            return BadRequest("ivalidmode");
        }
        [HttpGet("OrderChartData")]
        public async Task<IActionResult> GetOrdersData(int mode)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var applicationDbContext = new List<Order>();
            if (mode == 999)
            {
                applicationDbContext = await _context.Orders
                    .Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == seller.ShopId)
                    .ToListAsync();
            }
            else if (mode == 30)
            {
                applicationDbContext = await _context.Orders
                    .Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == seller.ShopId)
                    .Where(o => o.OrderDate.Date >= DateTime.Today.AddDays(-30))
                    .ToListAsync();
            }
            else if (mode == 365)
            {
                applicationDbContext = await _context.Orders
                    .Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == seller.ShopId)
                    .Where(o => o.OrderDate.Date >= DateTime.Today.AddDays(-365))
                    .ToListAsync();
            }
            var orders = applicationDbContext
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Quantity = g.Count(),
                })
                .ToList();

            return Ok(orders);
        }
    }
}
