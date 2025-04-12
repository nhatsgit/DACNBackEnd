using ecommerce_api.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerce_api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleClaimsController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimsController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model)
        {
            if (await _roleManager.RoleExistsAsync(model.RoleName))
                return BadRequest("Role đã tồn tại!");

            var role = new IdentityRole(model.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            foreach (var claim in model.Claims)
            {
                await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
            }

            return Ok($"Role '{model.RoleName}' được tạo thành công!");
        }
        [HttpPost("AssignClaims")]
        public async Task<IActionResult> AssignClaimsToRole([FromQuery] string roleName, [FromBody] List<UserClaimDto> claims)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Role không tồn tại!");

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var claimDto in claims)
            {
                var existingClaim = existingClaims.FirstOrDefault(c => c.Type == claimDto.Type);
                if (existingClaim != null)
                {
                    await _roleManager.RemoveClaimAsync(role, existingClaim);
                }

                await _roleManager.AddClaimAsync(role, new Claim(claimDto.Type, claimDto.Value));
            }

            return Ok($"Đã gán claims cho role '{roleName}' thành công!");
        }
        [HttpGet("GetClaims")]
        public async Task<IActionResult> GetClaimsByRole([FromQuery] string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Role không tồn tại!");

            var claims = await _roleManager.GetClaimsAsync(role);
            var claimList = claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(claimList);
        }
    }
}
