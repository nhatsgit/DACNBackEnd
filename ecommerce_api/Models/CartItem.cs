using ecommerce_api.Models;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        public int ShoppingCartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
