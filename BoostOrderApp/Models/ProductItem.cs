namespace BoostOrderApp.Models;

public sealed class ProductItem
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? ImageUrl { get; init; }
}
