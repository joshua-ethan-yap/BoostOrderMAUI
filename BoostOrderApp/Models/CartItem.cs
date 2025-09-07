using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoostOrderApp.Models;

public sealed class CartItem : INotifyPropertyChanged
{
    private int _quantity;
    
    public int ProductId { get; init; }
    public string Name { get; init; } = "";
    public string? ImageUrl { get; init; }
    public string Sku { get; init; } = "";
    public decimal UnitPriceValue { get; init; }
    public int StockQuantity { get; init; }
    public int ItemNumber { get; set; }

    public int Quantity 
    { 
        get => _quantity; 
        set 
        { 
            if (_quantity != value)
            {
                _quantity = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPriceValue));
                OnPropertyChanged(nameof(TotalPrice));
            }
        } 
    }

    // Computed properties for display
    public string UnitPrice => $"RM{UnitPriceValue:F2}";
    public decimal TotalPriceValue => UnitPriceValue * Quantity;
    public string TotalPrice => $"RM{TotalPriceValue:F2}";
    public string StockDisplay => $"{StockQuantity} In stock";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Static method to create from ProductItem
    public static CartItem FromProduct(ProductItem product, int quantity = 1)
    {
        return new CartItem
        {
            ProductId = product.Id,
            Name = product.Name,
            ImageUrl = product.ImageUrl,
            Sku = product.DisplaySku.Replace("SKU_", ""),
            UnitPriceValue = product.Price,
            StockQuantity = product.StockQuantity,
            Quantity = quantity
        };
    }
}