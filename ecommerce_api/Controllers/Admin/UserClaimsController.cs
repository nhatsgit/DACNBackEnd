using ecommerce_api.DTO;
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
        [HttpPost("claims")]
        public async Task<IActionResult> AssignClaimsToUser([FromQuery] string username, [FromBody] List<UserClaimDto> claims)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username là bắt buộc");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User không tồn tại");

            foreach (var claimDto in claims)
            {
                var existingClaims = await _userManager.GetClaimsAsync(user);
                var existingClaim = existingClaims.FirstOrDefault(c => c.Type == claimDto.Type);
                if (existingClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, existingClaim);
                }

                await _userManager.AddClaimAsync(user, new Claim(claimDto.Type, claimDto.Value));
            }

            return Ok("Gán quyền thành công");
        }
        [HttpGet("claims")]
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
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromQuery] string username, [FromQuery] string roleName)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User không tồn tại");

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                return BadRequest("Role không hợp lệ");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                return BadRequest("Gán role thất bại");

            return Ok($"Đã gán role '{roleName}' cho user '{username}'");
        }
    }

}
