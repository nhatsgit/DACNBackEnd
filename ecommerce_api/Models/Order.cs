using ecommerce_api.Models;
using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string? ShippingAddress { get; set; }

    public string? Notes { get; set; }

    public int? VoucherId { get; set; }

    public int? PaymentId { get; set; }

    public int? OrderStatusId { get; set; }

    public string UserId { get; set; } = null!;

    public int? OrderStatusId1 { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus? OrderStatus { get; set; }

    public virtual OrderStatus? OrderStatusId1Navigation { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}
