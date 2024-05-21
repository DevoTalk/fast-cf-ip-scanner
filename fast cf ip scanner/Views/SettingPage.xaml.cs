namespace fast_cf_ip_scanner.Views;

public partial class SettingPage : ContentPage
{
	public SettingPage(SettingViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}