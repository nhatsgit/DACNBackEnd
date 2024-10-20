using System;
using System.Collections.Generic;

namespace ecommerce_api.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string TenSp { get; set; } = null!;

    public string? AnhDaiDien { get; set; }

    public string? MoTa { get; set; }

    public string? ThongSo { get; set; }

    public decimal GiaNhap { get; set; }

    public decimal GiaBan { get; set; }

    public int SoLuongCon { get; set; }

    public int? PhanTramGiam { get; set; }

    public int? DiemDanhGia { get; set; }

    public bool DaAn { get; set; }

    public int ProductCategoryId { get; set; }

    public int BrandId { get; set; }

    public int ShopId { get; set; }

    public string? MaSp { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Category ProductCategory { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Shop Shop { get; set; } = null!;
}
