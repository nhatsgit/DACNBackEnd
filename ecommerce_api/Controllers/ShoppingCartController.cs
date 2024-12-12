using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Migrations;
using ecommerce_api.Models;
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
    [Authorize(Roles = "Customer,Developer")]


    public class ShoppingCartController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(EcomerceDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }
        [Authorize]
        [HttpPost("addToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto model)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            if (model == null || model.Quantity <= 0)
            {
                return BadRequest("Invalid product data.");
            }
            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return BadRequest("Invalid product data.");
            }

            var shoppingCart = await _context.ShoppingCart
                .Include(s => s.CartItems)
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.ShopId == product.ShopId);


            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    UserId = user.Id,
                    ShopId = product.ShopId,
                    CreatedDate = DateTime.UtcNow,
                };
                _context.ShoppingCart.Add(shoppingCart);
                await _context.SaveChangesAsync();
            }

            var cartItem = shoppingCart.CartItems
                .FirstOrDefault(ci => ci.ProductId == model.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += model.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                };
                shoppingCart.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added to cart successfully." });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyCarts()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            var mycarts = await _context.ShoppingCart.Where(s => s.UserId == user.Id).Include(s => s.CartItems).ThenInclude(c => c.Product).Include(s => s.Shop).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ShoppingCartDTO>>(mycarts));
        }
        [Authorize]
        [HttpGet("Vouchers")]
        public async Task<IActionResult> GetVouchers(int shopId)
        {
            var vouchers = await _context.Vouchers
                .Where(v => v.SoLuongCon > 0 && v.NgayHetHan > DateTime.Now && (v.NgayBatDau <= DateTime.Now || v.NgayBatDau == null) && (v.ShopId == shopId || v.ShopId == null))
                .ToListAsync();
           
            return Ok(vouchers);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            var cart = await _context.ShoppingCart.Where(s => s.UserId == user.Id&&s.ShoppingCartId==id).Include(s => s.CartItems).ThenInclude(c=>c.Product).Include(s=>s.Shop).FirstOrDefaultAsync();
            if (cart == null)
            {
                return BadRequest();
            }
            return Ok(_mapper.Map<ShoppingCartDTO>(cart));
        }
        [Authorize]
        [HttpPost("checkOut")]
        public async Task<IActionResult> Checkout([FromQuery]int shoppingCartId,CheckOutDTO model)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            var shoppingCart = await _context.ShoppingCart
             .Include(c => c.CartItems)
            .ThenInclude(c=>c.Product)
            .FirstOrDefaultAsync(c => c.ShoppingCartId == shoppingCartId && c.UserId == user.Id);

            if (shoppingCart == null)
            {
                return BadRequest("Invalid  data.");
                
            }
            var cartPrice = shoppingCart.CartItems.Sum(ci => ci.Quantity * (ci.Product.GiaBan * (100 - ci.Product.PhanTramGiam??0)/100 )) ;
            var totalPrice = cartPrice;
            var voucher=await _context.Vouchers.FirstOrDefaultAsync(v=>v.VoucherId==model.VoucherId);
            if (voucher!=null&& voucher.VoucherId != 1)
            {
                decimal decreasePrice = cartPrice / 100 * voucher.PhanTramGiam;
                if (decreasePrice > voucher.GiamToiDa && voucher.GiamToiDa > 0)
                {
                    decreasePrice = voucher.GiamToiDa ?? decreasePrice;
                }
                totalPrice = cartPrice - decreasePrice;
                voucher.SoLuongCon--;
            }
            foreach (var item in shoppingCart.CartItems)
            {
                Product _product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (_product.SoLuongCon > 0)
                {
                    _product.SoLuongCon -= (int)item.Quantity;
                }

            }
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                TotalPrice = totalPrice,
                ShippingAddress = model.ShippingAddress,
                OrderStatusId = 1,
                VoucherId=model.VoucherId??1,
                Notes=model.Notes??" ",
                PaymentId=model.PaymentId??1,
                UserId = user.Id,
                OrderDetails = shoppingCart.CartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price= (ci.Product.GiaBan*(100-ci.Product.PhanTramGiam)/100)?? ci.Product.GiaBan
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(shoppingCart.CartItems);
            _context.ShoppingCart.Remove(shoppingCart);

            await _context.SaveChangesAsync();

            
            return Ok(order);
        }
        [Authorize]
        [HttpPost("deleteItem")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var cartItem= await _context.CartItems.Include(c=>c.ShoppingCart).FirstOrDefaultAsync(c=>c.CartItemId==cartItemId);
            if (cartItem==null)
            {
                return BadRequest("Invalid Data");
            }
            var shoppingCart = await _context.CartItems.Where(c=>c.ShoppingCartId==cartItem.ShoppingCartId).ToListAsync();
            
            if (shoppingCart.Count <=1)
            {
                _context.CartItems.Remove(cartItem);
                _context.ShoppingCart.Remove(cartItem.ShoppingCart);
            }else
                _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
        [Authorize]
        [HttpPost("updateCartItem")]
        public async Task<IActionResult> UpdateCartItem(AddToCartDto addToCartDto)
        {
            var cartItem= await _context.CartItems.Include(c=>c.ShoppingCart).FirstOrDefaultAsync(c=>c.CartItemId== addToCartDto.ProductId);
            if (cartItem==null|| addToCartDto.Quantity == 0)
            {
                return BadRequest("Invalid Data");
            }
            
            cartItem.Quantity= addToCartDto.Quantity;
            await _context.SaveChangesAsync();
            return Ok("update successfully");
        }
        [Authorize]
        [HttpPost("deleteAll")]
        public async Task<IActionResult> RemoveAllCart()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            var shoppingCarts = await _context.ShoppingCart
                .Where(s => s.UserId == user.Id)
                .Include(s => s.CartItems)
                .ToListAsync();
            if (shoppingCarts == null)
            {
                return BadRequest("Invalid Data");
            }
            shoppingCarts.ForEach(cart => _context.CartItems.RemoveRange(cart.CartItems)); 
            _context.ShoppingCart.RemoveRange(shoppingCarts);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }


    }
}
