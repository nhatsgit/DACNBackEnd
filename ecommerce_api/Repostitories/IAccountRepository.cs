﻿using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;

namespace ecommerce_api.Repostitories
{
    public interface IAccountRepository
    {
        public Task<string> Login(LoginModel loginModel);
        public Task<IdentityResult> Register(RegisterModel registerModel, IFormFile? avatarImage);
        public Task<ApplicationUser> GetCurrentUserAsync(string userId);
        public Task<IdentityResult> SaveChangesUser(ApplicationUser user);
        public Task<bool> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword);
        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    }
}
