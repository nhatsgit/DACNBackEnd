using ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace ecommerce_api.Models
{

    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }

        // ID của người dùng
        public string UserId { get; set; } = null!;

        // ID của shop
        public int ShopId { get; set; }

        // Tổng giá trị giỏ hàng
        public decimal TotalPrice { get; set; }

        // Ngày tạo giỏ hàng
        public DateTime CreatedDate { get; set; }

        // Ngày cập nhật cuối
        public DateTime? UpdatedDate { get; set; }

        // Quan hệ 1-n với các CartItem
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Quan hệ tới Shop
        public virtual Shop Shop { get; set; } = null!;
    }
}
