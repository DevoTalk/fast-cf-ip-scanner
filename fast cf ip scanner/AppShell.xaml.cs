using fast_cf_ip_scanner.Views;

namespace fast_cf_ip_scanner;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SettingPage), typeof(SettingPage));
    }
}
