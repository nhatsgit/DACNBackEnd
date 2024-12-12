using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

    public class StaffController : ControllerBase
    {
        private readonly EcomerceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;


        public StaffController(UserManager<ApplicationUser> userManager,
            EcomerceDbContext context,
            RoleManager<IdentityRole> roleManager, IAccountRepository accountRepository, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _accountRepository = accountRepository;
            _mapper = mapper;


        }
        [HttpGet]
        public async Task<IActionResult> Staffs()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);

            var role = await _roleManager.FindByNameAsync("ShopStaff");
            var userIds = await _context.UserRoles
                                    .Where(ur => ur.RoleId == role.Id)
                                    .Select(ur => ur.UserId)
                                    .ToListAsync();
            var staffInShop = await _context.Users
                                    .Where(u => userIds.Contains(u.Id) && u.ShopId == user.ShopId)
                                    .ToListAsync();
            var userInfo = _mapper.Map<IEnumerable<UserDTO>>(staffInShop);

            return Ok(userInfo);
        }
        [HttpPost]
        public async Task<IActionResult> AddStaff(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userName == null)
                {
                    return Unauthorized();
                }
                var seller = await _accountRepository.GetCurrentUserAsync(userName);
                var user = new ApplicationUser
                {

                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    Avatar = model.Avatar,
                    ShopId = seller.ShopId,

                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("ShopStaff"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("ShopStaff"));
                    }

                    await _userManager.AddToRoleAsync(user, "ShopStaff");

                    return Ok("Đăng ký thành công");
                }
                else
                {
                    BadRequest(result);
                }
            }

            return BadRequest("Đăng Ký Không Thành Công");
        }
        [HttpPost("DisableStaff")]
        public async Task<IActionResult> DeleteStaff(string userName)
        {
            var staff = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (staff == null)
            {
                return NotFound();
            }
            staff.ShopId = null;
            await _context.SaveChangesAsync();
            return Ok("Thành công");
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetStaffPassword(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("User ID is required.");
            }
            var currentUserName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserName == null)
            {
                return Unauthorized();
            }
            var seller = await _accountRepository.GetCurrentUserAsync(userName);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null||user.ShopId!=seller.ShopId)
            {
                return NotFound("User not found.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "P@55W0rd");

            if (result.Succeeded)
            {

                return Ok("reset thành công");
            }

            return BadRequest(result.Errors);
        }
    }
}
