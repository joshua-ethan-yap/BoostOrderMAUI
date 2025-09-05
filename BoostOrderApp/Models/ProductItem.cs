using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoostOrderApp.Models;

public sealed class ProductItem : INotifyPropertyChanged
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? ImageUrl { get; init; }
    public string DisplaySku { get; init; } = "";
    public string DisplayPrice { get; init; } = "RM0.00";
    public string DisplayStock { get; init; } = "0 In stock";

    // UI-specific property that needs change notification
    private int _cartQuantity = 1;
    public int CartQuantity
    {
        get => _cartQuantity;
        set
        {
            if (_cartQuantity != value)
            {
                _cartQuantity = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}