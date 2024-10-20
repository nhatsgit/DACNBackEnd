using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string VoucherCode { get; set; } = null!;

    public int PhanTramGiam { get; set; }

    public DateTime NgayHetHan { get; set; }

    public int SoLuongCon { get; set; }

    public int VoucherCategoryId { get; set; }

    public decimal? GiamToiDa { get; set; }

    public decimal? DonToiThieu { get; set; }

    public int? ShopId { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual VoucherCategory VoucherCategory { get; set; } = null!;
}
