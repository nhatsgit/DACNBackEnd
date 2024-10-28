using ecommerce_api.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string? ShippingAddress { get; set; }

    public string? Notes { get; set; }
    [JsonIgnore]

    public int? VoucherId { get; set; }
    [JsonIgnore]

    public int? PaymentId { get; set; }
    [JsonIgnore]
    public int? OrderStatusId { get; set; }

    public string UserId { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus? OrderStatus { get; set; }


    public virtual Payment? Payment { get; set; }
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}
