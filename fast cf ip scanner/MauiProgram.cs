﻿using fast_cf_ip_scanner.Data;

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

		builder.Services.AddSingleton<MainPage>();
		
		builder.Services.AddSingleton<MainPageViewModel>();
		
		builder.Services.AddSingleton<IPServices>();

		builder.Services.AddSingleton<FastCFIPScannerDatabase>();

        return builder.Build();
	}
}
