using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result =await accountRepository.Register(model);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Succeeded);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result =await accountRepository.Login(model);
            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
