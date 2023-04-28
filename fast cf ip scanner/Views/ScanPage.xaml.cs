namespace fast_cf_ip_scanner.Views;

public partial class ScanPage : ContentPage
{
	public ScanPage(ScanPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}