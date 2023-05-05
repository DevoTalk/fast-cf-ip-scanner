
using fast_cf_ip_scanner.Views;

namespace fast_cf_ip_scanner.ViewModels
{
    public partial class ScanPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<IPModel> validIPs;

        [ObservableProperty]
        string maxPingOfIP;

        [ObservableProperty]
        bool startBtnEnable;

        [ObservableProperty]
        bool isBusy;

        readonly IPService _iPServices;
        readonly WorkerService _workerServices;
        public ScanPageViewModel(IPService iPServices,WorkerService workerService)
        {
            validIPs = new ObservableCollection<IPModel>();
            
            _iPServices = iPServices;
            _workerServices = workerService;

            this.Title = "scan";

        }
        

        [RelayCommand]
        async void GetValidIPs()
        {

            StartBtnEnable = false;

            IsBusy = true;

            List<IPModel> validIp = new List<IPModel>();

            ValidIPs.Clear();

            var maxping = ConvertMaxPingOfIPToInt(MaxPingOfIP);

            while (validIp.Count == 0)
            {
                validIp.AddRange(await _iPServices.GetIpValid(maxping));
            }

            validIp.Sort((x, y) => x.Ping.CompareTo(y.Ping));

            foreach (var ip in validIp)
            {
                ValidIPs.Add(ip);
            }

            IsBusy = false;

            StartBtnEnable = true;

        }
        int ConvertMaxPingOfIPToInt(string maxPing)
        {
            if (maxPing == null)
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
                var workers = await _workerServices.GetWorkers();

                string[] workersForShow;
                if(workers.Count == 0)
                {
                    workersForShow = new string[2];
                    workersForShow[1] = "Please add a worker";
                }
                else
                {
                    workersForShow = new string[workers.Count + 1];
                }
                workersForShow[0] = "Copy";
                
                for (int i = 1; i <= workers.Count; i++)
                {
                    workersForShow[i] = workers[i-1].Url;
                }
                var reslut = await App.Current.MainPage.DisplayActionSheet($"What to Do With {ipModel.IP}", null, null, workersForShow); ;

                if (reslut != null)
                {
                    if (reslut == "Copy")
                    {
                        await Clipboard.SetTextAsync(ipModel.IP);
                        await App.Current.MainPage.DisplayAlert("Copied", $"the {ipModel.IP} is copied", "OK");
                    }
                    else if(reslut == "Please add a worker")
                    {
                        await Shell.Current.GoToAsync(nameof(SettingPage));
                        return;
                    }
                    else
                    {
                        StartBtnEnable = false;
                        IsBusy = true;

                        var config = await _workerServices.GetConfigFromWorker(reslut, ipModel.IP);

                        StartBtnEnable = true;
                        IsBusy = false;

                        if (config != string.Empty)
                        {
                            await Clipboard.SetTextAsync(ipModel.IP);
                            await App.Current.MainPage.DisplayAlert("Copied", $"the Configs is copied", "OK");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Erorr", $"have a erorr try agane", "OK");
                        }
                    }
                }
            }
        }
    }
}
