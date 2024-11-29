using AutoMapper;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ecommerce_api.Controllers.Seller
{
    [Route("api/seller/[controller]")]
    [ApiController]
    [Authorize(Roles = "Developer,ShopOwner")]
    public class ShopsController : ControllerBase
    {
        // GET: api/<ShopsController>
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;


        public ShopsController(EcomerceDbContext context, IMapper mapper,IAccountRepository accountRepository)
        {
            _context = context;
            _mapper = mapper;
            _accountRepository = accountRepository;

        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);

            var shop = await _context.Shops.Where(c => c.ShopId == user.ShopId).FirstOrDefaultAsync();

            return Ok(shop);
        }

        // GET api/<ShopsController>/5
        

        // POST api/<ShopsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("haha")]
        public async Task<IActionResult> Edit( IFormFile imageAvatar, IFormFile imageBackground)
        {
            /*if (seller == null)
            {
                seller = await _userManager.GetUserAsync(User);
            }
            int id = seller.ShopId ?? 0;*/
            /*if (id == 0)
            {
                return NotFound();
            }
            if (id != shop.ShopId)
            {
                return NotFound();
            }
            try
            {
                if (imageAvatar != null)
                {
                    shop.AnhDaiDien = await UploadImage.SaveImage(imageAvatar);
                }

                if (imageBackground != null)
                {
                    shop.AnhBia = await UploadImage.SaveImage(imageBackground);
                }
                if (specificAddress != null && address != null)
                {
                    shop.DiaChi = specificAddress + ", " + address;
                }
                _context.Update(shop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Shops");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopExists(shop.ShopId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["ShopCategoryId"] = new SelectList(_context.ShopCategories, "ShopCategoryId", "ShopCategoryId", shop.ShopCategoryId);
            return View(shop);*/
            return Ok("xx");
        }

        // DELETE api/<ShopsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
