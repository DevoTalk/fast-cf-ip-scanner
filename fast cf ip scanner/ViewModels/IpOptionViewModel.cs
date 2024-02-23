﻿
using System.ComponentModel;
using System.Diagnostics.Tracing;

namespace fast_cf_ip_scanner.ViewModels;

public record PortForShow(string Port,bool IsChecked);


[QueryProperty("IpOptions", "IpOptions")]

public partial class IpOptionViewModel : BaseViewModel
{
    [ObservableProperty]
    IpOptionModel _ipOptions;

    [ObservableProperty]
    ObservableCollection<PortForShow> ports;
    
    [ObservableProperty]
    int maxPingOfIP;

    [ObservableProperty]
    int minimumCountOfValidIp;
    
    [ObservableProperty]
    int countOfRepeatTestForEachIp;


    bool Saved = false;


    public IpOptionViewModel()
    {
        ports = new ObservableCollection<PortForShow>();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == "IpOptions" && !Saved)
        {
            foreach (var port in IpOptions.Ports)
            {
                Ports.Add(new PortForShow(port, false));
            }
            MaxPingOfIP = IpOptions.MaxPingOfIP;
            MinimumCountOfValidIp = IpOptions.MinimumCountOfValidIp;
            CountOfRepeatTestForEachIp = IpOptions.CountOfRepeatTestForEachIp;
        }
    }

    [RelayCommand]
    async void Save()
    {
        Saved = true;
        IpOptions.Ports.Clear();
        foreach (var portForShow in Ports)
        {
            if (portForShow.IsChecked)
            {
                IpOptions.Ports.Add(portForShow.Port);
            }
        }
        if (IpOptions.Ports.Count == 0)
        {
            IpOptions.Ports.AddRange(GetRandomPort());
        }
        if (MinimumCountOfValidIp > 0)
        {
            IpOptions.MinimumCountOfValidIp = MinimumCountOfValidIp;
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("error", "Minimum Count is 1", "ok");
        }
        if (MinimumCountOfValidIp > 0)
        {
            IpOptions.CountOfRepeatTestForEachIp = CountOfRepeatTestForEachIp;
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("error", "Minimum Count is 1", "ok");

        }
        if (MaxPingOfIP > 100)
        {
            IpOptions.MaxPingOfIP = MaxPingOfIP;
        }
        else 
        {
            await App.Current.MainPage.DisplayAlert("error", "Minimum ping is 100", "ok");
        }

        await Shell.Current.GoToAsync("../");
    }
    public string[] GetRandomPort(int count = 3)
    {
        Random random = new Random();
        string[] randomPorts = new string[count];
        var allPorts = Constants.HttpPorts.Concat(Constants.HttpsPorts).ToList();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = random.Next(allPorts.Count);
            randomPorts[i] = allPorts[randomIndex];
        }
        return randomPorts;
    }


}
