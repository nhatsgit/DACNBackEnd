using ecommerce_api.Models;
using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class Review
{
    public int ReviewsId { get; set; }

    public string NoiDung { get; set; } = null!;

    public int? DiemDanhGia { get; set; }

    public DateTime ThoiGianDanhGia { get; set; }

    public string CustomerId { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual ApplicationUser Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ReviewsImage> ReviewsImages { get; set; } = new List<ReviewsImage>();
}
