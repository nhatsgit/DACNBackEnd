using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync(); // Lấy danh sách các role
            return Ok(roles);
        }
    }
}
