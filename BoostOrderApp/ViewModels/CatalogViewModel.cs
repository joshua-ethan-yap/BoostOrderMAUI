using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoostOrderApp.Models;
using BoostOrderApp.Services;
using Microsoft.Maui.Networking;

namespace BoostOrderApp.ViewModels;

public sealed class CatalogViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
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

    public CatalogViewModel(ApiService api)
    {
        _api = api;
        RefreshCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Products.Clear();
            Status = "Loading…";

            // Online check is informational; request will still throw if no network.
            var online = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            var (items, total, totalPages) = await _api.GetAllProductsAsync();

            var filtered = items.Where(p => string.Equals(p.Type, "variable", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var product in filtered)
            {
                Products.Add(new ProductItem
                {
                    Id = product.Id,
                    Name = product.Name ?? $"Product {product.Id}",
                    ImageUrl = product.Images?.FirstOrDefault()?.Src
                });
            }

            Status = $"{(online ? "ONLINE" : "UNKNOWN")} • Total:{total} • Pages:{totalPages} • Shown (variable):{Products.Count}";
        }
        catch (Exception ex)
        {
            Status = $"ERROR: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
