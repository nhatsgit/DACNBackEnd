using ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models
{

    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }

        public string UserId { get; set; } = null!;

        public int ShopId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        [JsonIgnore]
        public virtual Shop Shop { get; set; } = null!;
    }
}
