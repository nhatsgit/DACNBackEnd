using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string TenNhanHieu { get; set; } = null!;
    [JsonIgnore]

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
