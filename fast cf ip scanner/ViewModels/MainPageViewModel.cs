
namespace fast_cf_ip_scanner.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<IPModel> validIPs;

        [ObservableProperty]
        string maxPingOfIP;
        

        readonly IPServices _iPServices;

        public MainPageViewModel(IPServices iPServices)
        {
            validIPs = new ObservableCollection<IPModel>();
            _iPServices = iPServices;
        }


        [RelayCommand]
        async void GetValidIPs()
        {
            List<IPModel> validIp = new List<IPModel>();
            ValidIPs.Clear();
            var maxping = ConvertMaxPingOfIPToInt(MaxPingOfIP);
            while (validIp.Count == 0)
            {
                validIp.AddRange(await _iPServices.GetIpValid(maxping));
            }
            validIp.Sort((x,y)=>x.Ping.CompareTo(y.Ping));
            foreach (var ip in validIp)
            {
                ValidIPs.Add(ip);
            }
        }
        int ConvertMaxPingOfIPToInt(string maxPing) 
        {
            if(maxPing == null)
            {
                return 5000;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(maxPing);
                }
                catch
                {
                    return 5000;
                }
            }
            
        }

        [RelayCommand]
        async Task CopySelectedIPAsync(IPModel ipModel)
        {
            if (ipModel != null)
            {
                await Clipboard.SetTextAsync(ipModel.IP);
                await App.Current.MainPage.DisplayAlert("Copied", $"the {ipModel.IP} is copied", "OK");
            }

        }
    }
}
