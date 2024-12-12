using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Helper;
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
    [Authorize(Roles = "Developer,ShopOwner,ShopStaff")]
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


        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromForm] ShopDTO shopDTO, IFormFile? imageAvatar=null, IFormFile? imageBackground=null)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            if (user?.ShopId == null)
            {
                return BadRequest("Người dùng không thuộc shop nào.");
            }
            shopDTO.ShopId=user.ShopId.Value;

            try
            {
                if (imageAvatar != null)
                {
                    shopDTO.AnhDaiDien = await UploadImage.SaveImage(imageAvatar);
                }

                if (imageBackground != null)
                {
                    shopDTO.AnhBia = await UploadImage.SaveImage(imageBackground);
                }
                var shop=_mapper.Map<Shop>(shopDTO);
                _context.Update(shop);
                await _context.SaveChangesAsync();
                return Ok("cập nhật thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("đã xảy ra lỗi");
            }
        }

    }
}
