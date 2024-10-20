using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string TenNhanHieu { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
