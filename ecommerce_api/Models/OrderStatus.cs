using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class OrderStatus
{
    public int OrderStatusId { get; set; }

    public string TenTrangThai { get; set; } = null!;

    public virtual ICollection<Order> OrderOrderStatusId1Navigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderOrderStatuses { get; set; } = new List<Order>();
}
