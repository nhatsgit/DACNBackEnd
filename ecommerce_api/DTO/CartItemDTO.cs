using ecommerce_api.Models;

namespace ecommerce_api.DTO
{
    public class CartItemDTO
    {
        public int CartItemId { get; set; }

        public int Quantity { get; set; }

        public virtual ProductDTO Product { get; set; } = null!;
    }
}
