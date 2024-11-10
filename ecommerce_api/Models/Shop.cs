using ecommerce_api.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class Shop
{
    public int ShopId { get; set; }

    public string TenCuaHang { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string LienHe { get; set; } = null!;

    public string? AnhDaiDien { get; set; }

    public string? AnhBia { get; set; }

    public DateTime NgayTao { get; set; }

    public string MoTa { get; set; } = null!;

    public int ShopCategoryId { get; set; }

    public bool? BiChan { get; set; }
    [JsonIgnore]
    public virtual ICollection<ApplicationUser> AspNetUsers { get; set; } = new List<ApplicationUser>();
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    [JsonIgnore]
    public virtual ShopCategory ShopCategory { get; set; } = null!;
}
