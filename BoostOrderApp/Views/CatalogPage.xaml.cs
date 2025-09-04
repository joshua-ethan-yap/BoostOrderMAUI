namespace BoostOrderApp.Views;

public partial class CatalogPage : ContentPage
{
    public CatalogPage()
    {
        InitializeComponent();
    }

    private async void OnPing(object sender, EventArgs e)
    {
        await DisplayAlert("Ping", "Shell + page wiring OK.", "OK");
    }
}
