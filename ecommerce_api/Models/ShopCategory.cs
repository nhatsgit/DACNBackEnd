using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class ShopCategory
{
    public int ShopCategoryId { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
