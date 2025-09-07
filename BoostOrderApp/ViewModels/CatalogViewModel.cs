using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoostOrderApp.Models;
using BoostOrderApp.Services;
using Microsoft.Maui.Networking;

namespace BoostOrderApp.ViewModels;

public sealed class CatalogViewModel : INotifyPropertyChanged
{
    public string CategoryName { get; set; } = "Categories Name";
    public string CompanyName { get; set; } = "Company Name";
    private readonly ApiService _api;
    private readonly DatabaseService _database;
    private readonly CartViewModel _cartViewModel;
    private bool _isBusy;
    private string _status = "Idle";
    public ObservableCollection<ProductItem> Products { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get
        {
            return _isBusy;
        }
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public String Status
    {
        get
        {
            return _status;
        }
        private set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public Command RefreshCommand { get; }
    public Command<ProductItem> IncreaseQuantityCommand { get; }
    public Command<ProductItem> DecreaseQuantityCommand { get; }
    public Command<ProductItem> AddToCartCommand { get; }

    public CatalogViewModel(ApiService api, DatabaseService database, CartViewModel cartViewModel)
    {
        _api = api;
        _database = database;
        _cartViewModel = cartViewModel;

        RefreshCommand = new Command(async () => await LoadAsync());
        IncreaseQuantityCommand = new Command<ProductItem>(IncreaseQuantity);
        DecreaseQuantityCommand = new Command<ProductItem>(DecreaseQuantity);
        AddToCartCommand = new Command<ProductItem>(AddToCart);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Products.Clear();

            var online = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            // Try online first
            if (online)
            {
                Status = "Loading from server...";
                var (items, total, totalPages) = await _api.GetAllProductsAsync();

                var filtered = items.Where(p => string.Equals(p.Type, "variable", StringComparison.OrdinalIgnoreCase)).ToList();

                // Cache the data
                var entities = ConvertToEntities(filtered);
                await _database.SaveProductsAsync(entities);

                // Use fresh data
                PopulateFromApiData(items);
                Status = $"ONLINE • Total:{total} • Pages:{totalPages} • Shown (variable):{Products.Count}";
            }
            else
            {
                // Load from cache
                Status = "Loading from cache...";
                var cachedProducts = await _database.GetProductsAsync();

                PopulateFromCachedData(cachedProducts);

                Status = $"OFFLINE • Cached products: {cachedProducts.Count}";
            }
        }
        catch (Exception ex)
        {
            // If API fails, try cache as fallback
            var cachedProducts = await _database.GetProductsAsync();
            if (cachedProducts.Any())
            {
                PopulateFromCachedData(cachedProducts);
                Status = $"ERROR (showing cached): {ex.Message}";
            }
            else
            {
                Status = $"ERROR (no cache): {ex.Message}";
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private List<ProductEntity> ConvertToEntities(List<ProductDto> dtos)
    {
        return dtos.Select(dto => new ProductEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            Type = dto.Type,
            ImageUrl = dto.Images?.FirstOrDefault()?.Src,
            // Store the computed display values for offline use
            DisplaySku = dto.DisplaySku,
            DisplayPrice = dto.DisplayPrice,
            DisplayStock = dto.DisplayStock
        }).ToList();
    }

    private void PopulateFromApiData(List<ProductDto> items)
    {
        var variableProducts = items.Where(p => string.Equals(p.Type, "variable", StringComparison.OrdinalIgnoreCase));

        foreach (var product in variableProducts)
        {
            Products.Add(new ProductItem
            {
                Id = product.Id,
                Name = product.Name ?? $"Product {product.Id}",
                ImageUrl = product.ImageUrl,
                DisplaySku = product.DisplaySku,
                DisplayPrice = product.DisplayPrice,
                DisplayStock = product.DisplayStock
            });
        }
    }

    private void PopulateFromCachedData(List<ProductEntity> cachedItems)
    {
        foreach (var product in cachedItems)
        {
            Products.Add(new ProductItem
            {
                Id = product.Id,
                Name = product.Name ?? $"Product {product.Id}",
                ImageUrl = product.ImageUrl,
                DisplaySku = product.DisplaySku ?? "",
                DisplayPrice = product.DisplayPrice ?? "RM0.00",
                DisplayStock = product.DisplayStock ?? "0 In stock"
            });
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
    private void IncreaseQuantity(ProductItem product)
    {
        if (product != null)
        {
            product.CartQuantity++;
        }
    }

    private void DecreaseQuantity(ProductItem product)
    {
        if (product != null && product.CartQuantity > 1)
        {
            product.CartQuantity--;
        }
    }

    private void AddToCart(ProductItem product)
    {
        if (product != null)
        {
            _cartViewModel.AddItem(product, product.CartQuantity);
            product.CartQuantity = 1;

            Application.Current.MainPage.DisplayAlert("Added to Cart", $"{product.Name} added to cart", "OK");
        }
    }
}
