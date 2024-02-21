namespace fast_cf_ip_scanner.Views;

public partial class IpOptions : ContentPage
{
	public IpOptions(IpOptionModel ipOptions)
	{
		InitializeComponent();
		BindingContext = ipOptions;
	}
}