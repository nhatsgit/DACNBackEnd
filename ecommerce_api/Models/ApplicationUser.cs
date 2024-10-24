using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;

namespace ecommerce_api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string? Avatar { get; set; }
        public string? Description { get; set; }
        public int? ShopId { get; set; }
        public Shop? MyShop { get; set; }
        public List<Order>? Orders { get; set; }

    }
}
