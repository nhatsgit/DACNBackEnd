using ecommerce_api.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        [JsonIgnore]
        public int ShoppingCartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
        [JsonIgnore]
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;
        
        public virtual Product Product { get; set; } = null!;
    }
}
