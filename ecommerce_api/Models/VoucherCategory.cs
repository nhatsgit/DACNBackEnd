using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class VoucherCategory
{
    public int VoucherCategoryId { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
