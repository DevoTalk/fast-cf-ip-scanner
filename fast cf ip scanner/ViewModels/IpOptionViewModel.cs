
using System.ComponentModel;

namespace fast_cf_ip_scanner.ViewModels;


[QueryProperty("IpOptions", "IpOptions")]

public partial class IpOptionViewModel : BaseViewModel
{
    [ObservableProperty]
    IpOptionModel _ipOptions;
    




}
