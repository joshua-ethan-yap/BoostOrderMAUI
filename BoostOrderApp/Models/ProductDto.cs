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
    [JsonPropertyName("sku")] public string? Sku { get; set; }
    [JsonPropertyName("regular_price")] public string? RegularPrice { get; set; }
    [JsonPropertyName("stock_quantity")] public int? StockQuantity { get; set; }
    [JsonPropertyName("images")] public List<ProductImageDto>? Images { get; set; }
    [JsonPropertyName("variations")] public List<VariationDto>? Variations { get; set; }

    // UI properties - these will get the data from variations for variable products
    public int CartQuantity { get; set; } = 1;
    
    public string DisplaySku => GetFirstVariation()?.Sku ?? Sku ?? "";
    public string DisplayPrice => GetDisplayPriceFormatted();
    public string DisplayStock => GetDisplayStockFormatted();
    public string? ImageUrl => Images?.FirstOrDefault()?.Src;

    private VariationDto? GetFirstVariation() => Variations?.FirstOrDefault();

    private string GetDisplayPriceFormatted()
    {
        var priceString = GetFirstVariation()?.RegularPrice ?? RegularPrice ?? "0";
        if (decimal.TryParse(priceString, out var price))
            return $"RM{price:F2}";
        return "RM0.00";
    }

    private string GetDisplayStockFormatted()
    {
        // For variable products, sum up inventory from all branches
        var variation = GetFirstVariation();
        if (variation?.Inventory != null && variation.Inventory.Any())
        {
            var totalStock = variation.Inventory.Sum(inv => inv.StockQuantity);
            return $"{totalStock} In stock";
        }
        
        // Fallback to main product stock
        var stock = StockQuantity ?? 0;
        return $"{stock} In stock";
    }
}

public sealed class ProductImageDto
{
    [JsonPropertyName("src")] public string? Src { get; set; }
    [JsonPropertyName("src_small")] public string? SrcSmall { get; set; }
    [JsonPropertyName("src_medium")] public string? SrcMedium { get; set; }
    [JsonPropertyName("src_large")] public string? SrcLarge { get; set; }
}

public sealed class VariationDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("sku")] public string? Sku { get; set; }
    [JsonPropertyName("regular_price")] public string? RegularPrice { get; set; }
    [JsonPropertyName("stock_quantity")] public int? StockQuantity { get; set; }
    [JsonPropertyName("inventory")] public List<InventoryDto>? Inventory { get; set; }
}

public sealed class InventoryDto
{
    [JsonPropertyName("branch_id")] public int BranchId { get; set; }
    [JsonPropertyName("stock_quantity")] public int StockQuantity { get; set; }
    [JsonPropertyName("physical_stock_quantity")] public int PhysicalStockQuantity { get; set; }
}