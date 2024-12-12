using ecommerce_api.Models;

namespace ecommerce_api.DTO
{
    public class ShopDTO
    {
        public int ShopId { get; set; }

        public string TenCuaHang { get; set; } = null!;

        public string DiaChi { get; set; } = null!;

        public string LienHe { get; set; } = null!;

        public string? AnhDaiDien { get; set; }

        public string? AnhBia { get; set; }

        public DateTime NgayTao { get; set; }

        public string MoTa { get; set; } = null!;

        public int ShopCategoryId { get; set; }

        public bool? BiChan { get; set; }
    }
}
