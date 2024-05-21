namespace fast_cf_ip_scanner.Views;

public partial class IpOptionsPage : ContentPage
{
    readonly IpOptionViewModel vm;
    public IpOptionsPage(IpOptionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        vm = viewModel;
    }
}