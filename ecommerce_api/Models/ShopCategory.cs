using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class ShopCategory
{
    public int ShopCategoryId { get; set; }

    public string TenLoai { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
