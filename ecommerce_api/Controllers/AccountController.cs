using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Helper;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;
        public AccountController(IAccountRepository accountRepository,IMapper mapper)
        {
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model, IFormFile? avatarImage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result =await accountRepository.Register(model,avatarImage);
            
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
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }

            var user = await accountRepository.GetCurrentUserAsync(userName);
            if (user == null)
            {
                return NotFound();
            }

            var userInfo = mapper.Map<UserDTO>(user);

            return Ok(userInfo);
        }
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await accountRepository.GetCurrentUserAsync(userName);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            bool isOldPasswordCorrect = await accountRepository.CheckPasswordAsync(user, changePasswordDTO.OldPassword);

            if (!isOldPasswordCorrect)
            {
                return StatusCode(500, "Mật khẩu cũ không chính xác.");
            }

            var result = await accountRepository.ChangePasswordAsync(user,changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

            if (!result)
            {
                return StatusCode(500, "Có lỗi xảy ra khi thay đổi mật khẩu.");
            }

            return Ok("Mật khẩu đã được thay đổi thành công.");
        }
        [HttpPut("edit")]
        public async Task<IActionResult> EditUserInfo([FromForm] UserDTO userDTO, IFormFile? avatarImage)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized("Không xác định được người dùng.");
            }

            var user = await accountRepository.GetCurrentUserAsync(userName);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            try
            {
                if (!string.IsNullOrEmpty(userDTO.FullName))
                {
                    user.FullName = userDTO.FullName;
                }
                if (!string.IsNullOrEmpty(userDTO.Address))
                {
                    user.Address = userDTO.Address;
                }
                if (!string.IsNullOrEmpty(userDTO.PhoneNumber))
                {
                    user.PhoneNumber = userDTO.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(userDTO.Email))
                {
                    user.Email = userDTO.Email;
                }
                if (avatarImage != null)
                {
                    user.Avatar = await UploadImage.SaveImage(avatarImage);
                }
                else
                {
                    user.Avatar = "/images/avatar_default.png";
                }

                var result = await accountRepository.SaveChangesUser(user);
                if (!result.Succeeded)
                {
                    return BadRequest("Không thể cập nhật thông tin tài khoản.");
                }

                // Trả về thông tin đã cập nhật
                var updatedUserDTO = new UserDTO
                {
                    FullName = user.FullName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    UserName = user.UserName
                };

                return Ok(updatedUserDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra: {ex.Message}");
            }
        }


    }
}
