using ecommerce_api.DTO;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ecommerce_api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Developer")]
    public class ShopsController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ShopsController(UserManager<ApplicationUser> userManager,
            EcomerceDbContext context,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var role = await _roleManager.FindByNameAsync("ShopOwner");
            var userIds = await _context.UserRoles
                                    .Where(ur => ur.RoleId == role.Id)
                                    .Select(ur => ur.UserId)
                                    .ToListAsync();
            var shopOwners = await _context.Users
                                   .Where(u => userIds.Contains(u.Id))
                                   .Include(s => s.MyShop)
                                   .ToListAsync();
            
            return Ok(shopOwners);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var shopOwner = await _context.Users
                .Include(s => s.MyShop)
                .ThenInclude(s => s.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shopOwner == null)
            {
                return NotFound();
            }

            return Ok(shopOwner);
        }
        [HttpPost("resetPass")]
        public async Task<IActionResult> ResetShopPassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "P@55W0rd");

            if (result.Succeeded)
            {

                return Ok("thanh cong");
            }

            return BadRequest(result.Errors);
        }
        [HttpPost("block")]
        public async Task<IActionResult> BlockShop(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var shop = await _context.Shops
               .FirstOrDefaultAsync(m => m.ShopId == user.ShopId);
            shop.BiChan = true;
            await _context.SaveChangesAsync();
            return Ok("thanh cong");
            

        }
        [HttpPost("unblock")]
        public async Task<IActionResult> UnblockShop(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var shop = await _context.Shops
               .FirstOrDefaultAsync(m => m.ShopId == user.ShopId);
            shop.BiChan = false;
            await _context.SaveChangesAsync();
            return Ok("thanh cong");


        }
        [HttpPost]
        public async Task<IActionResult> AddShopOwner([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var shop = new Shop
                {
                    TenCuaHang = "newshop",
                    DiaChi = " ",
                    LienHe = " ",
                    AnhDaiDien = " ",
                    AnhBia = "  ",
                    NgayTao = DateTime.Now,
                    MoTa = " ",
                    ShopCategoryId = 1
                };

                _context.Shops.Add(shop);
                await _context.SaveChangesAsync();
                var user = new ApplicationUser
                {

                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    Avatar = model.Avatar,
                    ShopId = shop.ShopId,
                    PhoneNumber=model.PhoneNumber,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("ShopOwner"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("ShopOwner"));
                    }

                    await _userManager.AddToRoleAsync(user, "ShopOwner");

                    return Ok("thành công");
                }
                else
                {
                    BadRequest("Đăng Ký Không Thành Công");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest("Error");
        }

    }
}
