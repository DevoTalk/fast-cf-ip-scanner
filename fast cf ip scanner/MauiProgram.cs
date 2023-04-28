using fast_cf_ip_scanner.Data;
using fast_cf_ip_scanner.Views;

namespace fast_cf_ip_scanner;

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


        #region views
        
		builder.Services.AddSingleton<MainPage>();
		
		builder.Services.AddSingleton<ScanPage>();

		builder.Services.AddSingleton<SettingPage>();

        #endregion

        #region ViewModels
        
		builder.Services.AddSingleton<ScanPageViewModel>();

        builder.Services.AddSingleton<MainPageViewModel>();

        builder.Services.AddSingleton<SettingViewModel>();

        #endregion

        #region Services

        builder.Services.AddSingleton<IPService>();

        builder.Services.AddSingleton<WorkerService>();

        #endregion

        builder.Services.AddSingleton<FastCFIPScannerDatabase>();

        return builder.Build();
	}
}
