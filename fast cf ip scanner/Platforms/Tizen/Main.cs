using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace fast_cf_ip_scanner;

class Program : MauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
