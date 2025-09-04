using BoostOrderApp.Services;
using BoostOrderApp.ViewModels;
using BoostOrderApp.Views;
using Microsoft.Extensions.Logging;

namespace BoostOrderApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Services
		builder.Services.AddSingleton<ApiService>();

		// ViewModels
		builder.Services.AddSingleton<CatalogViewModel>();

		// Views
		builder.Services.AddSingleton<CatalogPage>();
		builder.Services.AddSingleton<CartPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
