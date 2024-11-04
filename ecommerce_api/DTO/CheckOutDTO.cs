using ecommerce_api.Models;

namespace ecommerce_api.DTO
{
    public class CheckOutDTO
    {
        public string? ShippingAddress { get; set; }

        public string? Notes { get; set; }

        public int? VoucherId { get; set; }

        public int? PaymentId { get; set; }
           
    }
}
