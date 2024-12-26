using System.Text.Json.Serialization;

namespace ecommerce_api.Models;

public partial class ProductFeature
{
    public int FeatureId { get; set; }

    public int ProductId { get; set; }

    public string FeatureVector { get; set; } = null!;

    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;
}