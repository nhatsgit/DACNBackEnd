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
    [Authorize(Roles = "Developer,ShopOwner,ShopStaff")]

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
                    .Count(o => o.OrderStatusId == 1 && o.OrderDate.Date >= DateTime.Today.AddDays(-7));
                var DonDaThanhToan = ordersshop
                    .Count(o => o.OrderStatusId == 5 && o.OrderDate.Date >= DateTime.Today.AddDays(-7));
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
                    .Count(o => o.OrderStatusId == 1 && o.OrderDate.Date >= DateTime.Today.AddDays(-30));
                var DonDaThanhToan = ordersshop
                    .Count(o => o.OrderStatusId == 5 && o.OrderDate.Date >= DateTime.Today.AddDays(-30));
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
        [HttpGet("RevenueAnalyze")]
        public async Task<IActionResult> RevenueAnalyze(int mode)
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
                var doanhThu = ordersshop
                .Where(o => o.OrderStatusId == 5)
                .Sum(o => o.TotalPrice);
                return Ok(doanhThu);
            }else if (mode == 0)
            {
                var doanhThu = ordersshop
                .Where(o => o.OrderStatusId == 5 && o.OrderDate.Date == DateTime.Today)
                .Sum(o => o.TotalPrice);
                return Ok(doanhThu);

            }
            else if (mode == 1)
            {
                var doanhThu = ordersshop
                .Where(o => o.OrderStatusId == 5 && o.OrderDate.Date == DateTime.Today.AddDays(-1))
                .Sum(o => o.TotalPrice);
                return Ok(doanhThu);

            }
            else if (mode == 7)
            {
                var doanhThu = ordersshop
                .Where(o => o.OrderStatusId == 5 && o.OrderDate.Date >= DateTime.Today.AddDays(-7))
                .Sum(o => o.TotalPrice);
                return Ok(doanhThu);

            }
            else if (mode == 30)
            {
                var doanhThu = ordersshop
                .Where(o => o.OrderStatusId == 5 && o.OrderDate.Date >= DateTime.Today.AddDays(-30))
                .Sum(o => o.TotalPrice);
                return Ok(doanhThu);

            }


            return NotFound("ivalidmode");
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
        [HttpGet("RevenueChartData")]
        public async Task<IActionResult> GetRevenuesData(int mode)
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
                    .Where(o => o.OrderStatusId == 5)
                    .ToListAsync();
            }
            else if (mode == 30)
            {
                applicationDbContext = await _context.Orders
                    .Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == seller.ShopId)
                    .Where(o => o.OrderDate.Date >= DateTime.Today.AddDays(-30))
                    .Where(o => o.OrderStatusId == 5)
                    .ToListAsync();
            }
            else if (mode == 365)
            {
                applicationDbContext = await _context.Orders
                    .Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == seller.ShopId)
                    .Where(o => o.OrderDate.Date >= DateTime.Today.AddDays(-365))
                    .Where(o => o.OrderStatusId == 5)
                    .ToListAsync();
            }
            var orders = applicationDbContext
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .ToList();

            return Ok(orders);
        }
        [HttpGet("ProductCategoryChartData")]

        public async Task<IActionResult> GetProductCategoryChartData()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var categories = _context.Categories
             .Where(c => c.Products != null && c.Products.Any(p => p.ShopId == seller.ShopId && p.SoLuongCon != 0 && p.DaAn != true))
             .Include(p => p.Products)
             .ToList();


            var data = categories.Select(c => c.Products.Count(p => p.ShopId == seller.ShopId && p.SoLuongCon != 0 && p.DaAn != true)).ToList();
            var labels = categories.Select(c => c.TenLoai).ToList();

            var chartData = new { data = data, labels = labels };

            return Ok(chartData);
        }
        [HttpGet("TopSaleProducts")]
        public async Task<IActionResult> GetTopSaleProduct()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            var topSaleProducts = _context.Products.Where(p => p.ShopId == seller.ShopId)
            .Select(p => new
            {
                p.TenSp,
                p.SoLuongCon,
                p.AnhDaiDien,
                p.PhanTramGiam,
                p.GiaBan,
                p.DiemDanhGia,
                SoLuongBan = p.OrderDetails.Sum(o => o.Quantity)
            })
            .OrderByDescending(p => p.SoLuongBan)
            .Take(5)
            .Where(p => p.SoLuongBan != 0)
            .ToList();
            return Ok(topSaleProducts);
        }
        [HttpGet("TopRevenueProducts")]

        public async Task<IActionResult> GetTopRevenueProduct()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);
            
            var topRevenueProducts = _context.Products.Where(p => p.ShopId == seller.ShopId)
            .Select(p => new
            {
                p.TenSp,
                p.SoLuongCon,
                p.AnhDaiDien,
                p.PhanTramGiam,
                p.GiaBan,
                p.DiemDanhGia,
                DoanhThu = p.OrderDetails.Where(o => o.Order.OrderStatusId == 5).Sum(o => o.Product.GiaBan * o.Quantity / 100 * (100 - o.Order.Voucher.PhanTramGiam))
            })
            .OrderByDescending(p => p.DoanhThu)
            .Take(5)
            .Where(p => p.DoanhThu != 0)
            .ToList();
            return Ok(topRevenueProducts);
        }

    }
}
