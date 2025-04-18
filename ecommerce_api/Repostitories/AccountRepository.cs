﻿using ecommerce_api.Helper;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ecommerce_api.Repostitories
{

    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountRepository(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,IConfiguration configuration,RoleManager<IdentityRole> roleManager) {
            this._userManager=userManager;
            this._signInManager=signInManager;
            this._config=configuration;
            this.roleManager = roleManager;

        }
        public async Task<string> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return string.Empty;
            }

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            authClaims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));

                var roleObj = await roleManager.FindByNameAsync(role);
                if (roleObj != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(roleObj);

                    foreach (var claim in roleClaims)
                    {
                        if (!authClaims.Any(c => c.Type == claim.Type))
                        {
                            authClaims.Add(claim);
                        }
                    }
                }
            }

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(180),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IdentityResult> Register(RegisterModel registerModel, IFormFile? avatarImage)
        {
            var user = new ApplicationUser {
                UserName=registerModel.UserName,
                FullName = registerModel.FullName,
                Address = registerModel.Address,
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
            };
            if (avatarImage != null)
            {
                user.Avatar = await UploadImage.SaveImage(avatarImage);
            }
            else
            {
                user.Avatar = "/images/avatar_default.png";
            }
            var result= await _userManager.CreateAsync(user,registerModel.Password);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("Customer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Customer"));
                }

                await _userManager.AddToRoleAsync(user, "Customer");
            }
            return result;
        }
        public async Task<ApplicationUser> GetCurrentUserAsync(string userId)
        {
            return await _userManager.FindByNameAsync(userId);
        }

        public async Task<IdentityResult> SaveChangesUser(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<bool> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            return result.Succeeded;
        }
    }
}
