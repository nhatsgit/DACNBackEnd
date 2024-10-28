using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class OrderStatus
{
    public int OrderStatusId { get; set; }

    public string TenTrangThai { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Order> OrderOrderStatuses { get; set; } = new List<Order>();
}
