using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class ProductImage
{
    public int ProductImageId { get; set; }

    public string Url { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
