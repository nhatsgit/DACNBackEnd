using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class ReviewsImage
{
    public int ReviewsImageId { get; set; }

    public string Url { get; set; } = null!;

    public int ReviewsId { get; set; }
    [JsonIgnore]
    public virtual Review Reviews { get; set; } = null!;
}
