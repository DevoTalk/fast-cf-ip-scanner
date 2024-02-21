namespace fast_cf_ip_scanner.Views;

public partial class IpOptionsPage : ContentPage
{

    public IpOptionsPage(IpOptionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}