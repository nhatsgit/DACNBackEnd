using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class ReviewsImage
{
    public int ReviewsImageId { get; set; }

    public string Url { get; set; } = null!;

    public int ReviewsId { get; set; }

    public virtual Review Reviews { get; set; } = null!;
}
