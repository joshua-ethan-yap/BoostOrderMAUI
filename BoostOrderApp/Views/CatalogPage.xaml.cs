using BoostOrderApp.ViewModels;

namespace BoostOrderApp.Views;

public partial class CatalogPage : ContentPage
{
    private readonly CatalogViewModel _viewModel;

    public CatalogPage(CatalogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Products.Count == 0)
            await _viewModel.LoadAsync();
    }

    private async void Reload_Clicked(object sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }
}
