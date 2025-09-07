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
		builder.Services.AddSingleton<DatabaseService>();

		// ViewModels
		builder.Services.AddSingleton<CatalogViewModel>();
		builder.Services.AddSingleton<CartViewModel>();

		// Views
		builder.Services.AddTransient<CatalogPage>();
		builder.Services.AddTransient<CartPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
