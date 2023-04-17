namespace fast_cf_ip_scanner.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<IPModel> validIPs;
        readonly IPServices _iPServices;
        public MainPageViewModel(IPServices iPServices)
        {
            validIPs = new ObservableCollection<IPModel>();
            _iPServices = iPServices;
        }

        [RelayCommand]
        void GetValidIPs()
        {
            List<IPModel> validIp = new List<IPModel>();
            while (validIp.Count == 0)
            {
                validIp.AddRange(_iPServices.GetIpValid());
            }

            foreach (var ip in validIp)
            {
                ValidIPs.Add(ip);
            }
        }
    }
}
