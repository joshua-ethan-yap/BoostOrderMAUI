using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoostOrderApp.Models;

namespace BoostOrderApp.ViewModels;

public sealed class CartViewModel : INotifyPropertyChanged
{
    public ObservableCollection<CartItem> CartItems { get; } = new();
    
    public string CompanyName { get; set; } = "Company Name";

    public Command<CartItem> RemoveItemCommand { get; }
    public Command ClearCartCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CartViewModel()
    {
        RemoveItemCommand = new Command<CartItem>(RemoveItem);
        ClearCartCommand = new Command(ClearCart);
        
        // Subscribe to collection changes to update totals
        CartItems.CollectionChanged += (s, e) => UpdateTotals();
    }

    // Computed properties for totals
    public decimal CartTotalValue => CartItems.Sum(item => item.TotalPriceValue);
    public string CartTotal => $"RM{CartTotalValue:F2}";
    public int ItemCount => CartItems.Count;
    public string CartSummary => $"Total ({ItemCount})";

    public void AddItem(ProductItem product, int quantity = 1)
    {
        // Check if item already exists
        var existingItem = CartItems.FirstOrDefault(item => item.ProductId == product.Id);
        
        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += quantity;
        }
        else
        {
            // Add new item
            var cartItem = CartItem.FromProduct(product, quantity);
            CartItems.Add(cartItem);
        }
        
        UpdateItemNumbers();
        UpdateTotals();
    }

    private void RemoveItem(CartItem item)
    {
        if (item != null && CartItems.Contains(item))
        {
            CartItems.Remove(item);
            UpdateItemNumbers();
            UpdateTotals();
        }
    }

    private async void ClearCart()
    {
        // Show confirmation dialog
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Clear Cart", 
            "Are you sure you want to clear all items from your cart?", 
            "Yes", 
            "Cancel");
        
        if (confirm)
        {
            CartItems.Clear();
            UpdateTotals();
        }
    }

    private void UpdateItemNumbers()
    {
        for (int i = 0; i < CartItems.Count; i++)
        {
            CartItems[i].ItemNumber = i + 1;
        }
    }

    private void UpdateTotals()
    {
        OnPropertyChanged(nameof(CartTotalValue));
        OnPropertyChanged(nameof(CartTotal));
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(CartSummary));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}