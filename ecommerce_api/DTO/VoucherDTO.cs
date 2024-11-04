namespace ecommerce_api.DTO
{
    public class VoucherDTO
    {
        public int VoucherId { get; set; }

        public string VoucherCode { get; set; } = null!;

        public int PhanTramGiam { get; set; }

        public DateTime NgayHetHan { get; set; }

        public int SoLuongCon { get; set; }

        public decimal? GiamToiDa { get; set; }

        public decimal? DonToiThieu { get; set; }

        public int? ShopId { get; set; }

        public DateTime? NgayBatDau { get; set; }
    }
}
