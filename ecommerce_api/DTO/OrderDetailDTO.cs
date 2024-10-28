using ecommerce_api.Models;

namespace ecommerce_api.DTO
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool? IsReview { get; set; }

        public virtual ProductDTO Product { get; set; } = null!;
    }
}
