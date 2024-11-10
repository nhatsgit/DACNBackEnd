using ecommerce_api.Models;

namespace ecommerce_api.DTO
{
    public class ShoppingCartDTO
    {
        public int ShoppingCartId { get; set; }
        public int ShopId { get; set; }
        public virtual ICollection<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
        public virtual Shop Shop { get; set; } = null!;
    }
}
