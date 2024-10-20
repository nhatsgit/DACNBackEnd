using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class Category
{
    public int ProductCategoryId { get; set; }

    public string? TenLoai { get; set; }

    public string? AnhDaiDien { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
