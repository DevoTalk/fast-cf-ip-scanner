
using fast_cf_ip_scanner.Views;
using System;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

namespace fast_cf_ip_scanner.ViewModels
{
    public partial class ScanPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<IPModel> validIPs;

        [ObservableProperty]
        bool exportBtnVisible = false;

        [ObservableProperty]
        bool isBusy = false;

        IpOptionModel _ipOption;

        readonly IPService _iPServices;
        readonly WorkerService _workerServices;
        public ScanPageViewModel(IPService iPServices, WorkerService workerService)
        {
            validIPs = new ObservableCollection<IPModel>();

            _iPServices = iPServices;
            _workerServices = workerService;

            _ipOption = new IpOptionModel();
        }

        [RelayCommand]
        async void GetValidIPs()
        {
            var protocols = new string[] { "Http test (recommended)", "TCP test", "Terminal Ping test" };

            var selectedProtocol =
                await App.Current.MainPage.DisplayActionSheet("which protocol", "Cancel", null, protocols);

            if (selectedProtocol == "Cancel" || selectedProtocol == null)
            {
                return;
            }

            IsBusy = true;
            ExportBtnVisible = false;

            List<IPModel> validIp = new List<IPModel>();

            ValidIPs.Clear();

            var ips = await _iPServices.GetIps();
            if (ips.Length < 1)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"have a error try again ", "OK");
            }
            else
            {
                while (validIp.Count == 0)
                {
                    validIp.AddRange(await _iPServices.GetIpValid(ips, _ipOption, selectedProtocol));
                }

                validIp.Sort((x, y) => x.Ping.CompareTo(y.Ping));

                foreach (var ip in validIp)
                {
                    ValidIPs.Add(ip);
                }
                await _iPServices.AddValidIpToDb(validIp);
            }
            IsBusy = false;
            ExportBtnVisible = true;
        }

        [RelayCommand]
        async Task ShowOptionsForSearchIp()
        {
            await Shell.Current.GoToAsync($"{nameof(IpOptionsPage)}", new Dictionary<string, object>
            {
                {"IpOptions",_ipOption},
            });
        }

        [RelayCommand]
        async Task ExportAllIPsToClipboard()
        {
            var ips = string.Join("\n", ValidIPs.Select(ip => ip.IP.ToString()));
            await Clipboard.SetTextAsync(ips);
            await App.Current.MainPage.DisplayAlert("Copied", $"the ips is copied", "OK");
        }
        [RelayCommand]
        async Task ShowSelectedIPOption(IPModel ipModel)
        {
            if (ipModel != null)
            {
                var workers = await _workerServices.GetWorkers();

                string[] Options;
                if (workers.Count == 0)
                {
                    Options = new string[3];
                    Options[2] = "Please add a worker";
                }
                else
                {
                    Options = new string[workers.Count + 2];
                }
                Options[0] = "Copy";

                Options[1] = "speed test";

                for (int i = 2; i <= workers.Count; i++)
                {
                    Options[i] = workers[i - 2].Url;
                }
                var reslut = await App.Current.MainPage.DisplayActionSheet($"What to Do With {ipModel.IP}", null, null, Options);

                if (reslut != null)
                {
                    if (reslut == "Copy")
                    {
                        var ip = ipModel.IP.Split("/")[0];
                        await Clipboard.SetTextAsync(ip);
                        await App.Current.MainPage.DisplayAlert("Copied", $"the {ip} is copied", "OK");
                    }
                    else if (reslut == "speed test")
                    {
                        IsBusy = true;
                        var downloadSizeForSpeedTestToMB = _ipOption.DownloadSizeForSpeedTest * 1024 * 1024;
                        var speed = await _iPServices.GetDownloadSpeedAsync($"https://speed.cloudflare.com/__down?bytes={downloadSizeForSpeedTestToMB}", ipModel.IP);
                        IsBusy = false;
                        await App.Current.MainPage.DisplayAlert("speed", $"{speed} Mb", "ok");
                    }
                    else if (reslut == "Please add a worker")
                    {
                        await Shell.Current.GoToAsync(nameof(SettingPage));
                        return;
                    }
                    else
                    {
                        IsBusy = true;

                        var config = await _workerServices.GetConfigFromWorker(reslut, ipModel.IP);

                        IsBusy = false;

                        if (config != string.Empty)
                        {
                            await Clipboard.SetTextAsync(config);
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
