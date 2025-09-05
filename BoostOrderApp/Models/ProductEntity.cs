
using SQLite;

namespace BoostOrderApp.Models;

[Table("Products")]
public sealed class ProductEntity
{
    [PrimaryKey]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? ImageUrl { get; set; }
    public string? DisplaySku { get; set; }
    public string? DisplayPrice { get; set; }
    public string? DisplayStock { get; set; }
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
}