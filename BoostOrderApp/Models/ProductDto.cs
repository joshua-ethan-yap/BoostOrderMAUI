using System.Text.Json.Serialization;

namespace BoostOrderApp.Models;

public sealed class ProductsResponse
{
    [JsonPropertyName("products")]
    public List<ProductDto>? Products { get; set; }
}

public sealed class ProductDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("images")] public List<ProductImageDto>? Images { get; set; }
    [JsonPropertyName("variations")] public List<VariationDto>? Variations { get; set; }
}

public sealed class ProductImageDto
{
    [JsonPropertyName("src")] public string? Src { get; set; }
}

public sealed class VariationDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("price")] public string? Price { get; set; }
    [JsonPropertyName("regular_price")] public string? RegularPrice { get; set; }
}
