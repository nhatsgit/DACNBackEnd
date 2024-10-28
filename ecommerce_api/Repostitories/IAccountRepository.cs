using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;

namespace ecommerce_api.Repostitories
{
    public interface IAccountRepository
    {
        public Task<string> Login(LoginModel loginModel);
        public Task<IdentityResult> Register(RegisterModel registerModel);
        public Task<ApplicationUser> GetCurrentUserAsync(string userId);

    }
}
