﻿
using fast_cf_ip_scanner.Views;
using System;
using System.ComponentModel;

namespace fast_cf_ip_scanner.ViewModels
{
    public partial class ScanPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<IPModel> validIPs;


        [ObservableProperty]
        bool isBusy = false;


        

        IpOptionModel _ipOption;
        
        readonly IPService _iPServices;
        readonly WorkerService _workerServices;
        public ScanPageViewModel(IPService iPServices,WorkerService workerService)
        {
            validIPs = new ObservableCollection<IPModel>();
            
            _iPServices = iPServices;
            _workerServices = workerService;

            _ipOption = new IpOptionModel();
        }

        [RelayCommand]
        async void GetValidIPs()
        {
            var protocols = new string[] { "Http test", "TCP test", "Terminal Ping test" };

            var selectedProtocol = 
                await App.Current.MainPage.DisplayActionSheet("which protocol", "Cancel", null, protocols);
            
            if(selectedProtocol == "Cancel" || selectedProtocol == null)
            {
                return;
            }

            IsBusy = true;

            List<IPModel> validIp = new List<IPModel>();

            ValidIPs.Clear();

            var ips = await _iPServices.GetIps();
            if (ips.Length < 1)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"have a error try again ","OK");
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
        async Task ShowSelectedIPOption(IPModel ipModel)
        {
            var workerss = await _workerServices.GetWorkers();

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
                var reslut = await App.Current.MainPage.DisplayActionSheet($"What to Do With {ipModel.IP}", null, null, workersForShow);

                if (reslut != null)
                {
                    if (reslut == "Copy")
                    {
                        var ip = ipModel.IP.Split("/")[0];
                        await Clipboard.SetTextAsync(ip);
                        await App.Current.MainPage.DisplayAlert("Copied", $"the {ip} is copied", "OK");
                    }
                    else if(reslut == "Please add a worker")
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
