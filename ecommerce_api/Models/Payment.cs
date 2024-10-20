using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
