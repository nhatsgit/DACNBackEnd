namespace ecommerce_api.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string TenSp { get; set; } = null!;
        public string? AnhDaiDien { get; set; }
        public string? MoTa { get; set; }
        public string? ThongSo { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongCon { get; set; }
        public int? PhanTramGiam { get; set; }
        public int? DiemDanhGia { get; set; }
        public string? MaSp { get; set; }
        public bool DaAn { get; set; }

        public int ProductCategoryId { get; set; }

        public int BrandId { get; set; }

        public int ShopId { get; set; }
    }
}
