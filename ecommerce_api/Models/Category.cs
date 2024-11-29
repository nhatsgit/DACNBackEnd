using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class Category
{
    public int ProductCategoryId { get; set; }

    public string? TenLoai { get; set; }

    public string? AnhDaiDien { get; set; }
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
