using ecommerce_api.DTO;
using ecommerce_api.Helper;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ecommerce_api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserClaimsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserClaimsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Roles = roles
                });
            }

            return Ok(userList);
        }
        [HttpGet("GetSystemClaims")]
        public IActionResult GetAllClaims()
        {
            var claims = ClaimTypesConstants.AllPolicies
                .Select(c => new
                {
                    Type = c.Key,
                    Description = c.Value
                }).ToList();

            return Ok(claims);
        }
        [HttpPost("AssignClaims")]
        public async Task<IActionResult> AssignClaimsToUser([FromQuery] string username, [FromBody] List<UserClaimDto> claims)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username là bắt buộc");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User không tồn tại");

            // Xóa toàn bộ claims hiện có
            var existingClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in existingClaims)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            // Gán các claims mới
            foreach (var claimDto in claims)
            {
                await _userManager.AddClaimAsync(user, new Claim(claimDto.Type, claimDto.Value));
            }

            return Ok("Gán quyền thành công");
        }
        [HttpGet("UserClaimsDetails")]
        public async Task<IActionResult> GetUserClaims([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username là bắt buộc");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User không tồn tại");

            var claims = await _userManager.GetClaimsAsync(user);
            return Ok(claims.Select(c => new { c.Type, c.Value }));
        }
        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("Username là bắt buộc.");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User không tồn tại.");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
        [HttpPost("AssignRoles")]
        public async Task<IActionResult> AssignRolesToUser([FromQuery] string username, [FromBody] List<string> userRole)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("Username là bắt buộc.");

            if (userRole == null || !userRole.Any())
                return BadRequest("Danh sách role không được rỗng.");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User không tồn tại.");

            // Xóa tất cả role hiện tại
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return BadRequest("Không thể xóa các role hiện tại.");

            // Kiểm tra từng role trước khi gán
            foreach (var role in userRole)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    return BadRequest($"Role '{role}' không tồn tại.");
            }

            var addResult = await _userManager.AddToRolesAsync(user, userRole);
            if (!addResult.Succeeded)
                return BadRequest("Không thể gán role mới.");

            return Ok($"Đã cập nhật roles cho user '{username}': {string.Join(", ", userRole)}");
        }
    }

}
