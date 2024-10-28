using ecommerce_api.Models;

namespace ecommerce_api.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string? ShippingAddress { get; set; }

        public string? Notes { get; set; }

        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();

        public virtual OrderStatus? OrderStatus { get; set; }
        public virtual Payment? Payment { get; set; }
        public virtual UserDTO User { get; set; } = null!;
        public virtual Voucher? Voucher { get; set; }
    }
}
