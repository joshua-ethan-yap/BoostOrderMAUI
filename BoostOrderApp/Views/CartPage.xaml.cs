using BoostOrderApp.ViewModels;

namespace BoostOrderApp.Views;

public partial class CartPage : ContentPage
{
    public CartPage(CartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}