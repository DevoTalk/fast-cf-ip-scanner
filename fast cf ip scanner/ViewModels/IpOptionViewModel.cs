
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

    [ObservableProperty]
    int countOfIpRanges;


    bool saved = false;


    public IpOptionViewModel()
    {
        ports = new ObservableCollection<PortForShow>();
    }

    [ObservableProperty]
    int countOfIpForTest;

    int totalTestForEachIp = 0;
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == "IpOptions" && !saved)
        {
            var allPorts = Constants.HttpPorts.Concat(Constants.HttpsPorts);
            var selectedPort = IpOptions.Ports;
            foreach (var port in allPorts)
            {
                Ports.Add(new PortForShow(port, selectedPort.Any(p => p == port)));
            }
            MaxPingOfIP = IpOptions.MaxPingOfIP;
            MinimumCountOfValidIp = IpOptions.MinimumCountOfValidIp;
            CountOfRepeatTestForEachIp = IpOptions.CountOfRepeatTestForEachIp;
            CountOfIpRanges = IpOptions.CountOfIpRanges;
            CountOfIpForTest = IpOptions.CountOfIpForTest;
        }
    }

    [RelayCommand]
    async void Save()
    {
        saved = true;
        IpOptions.Ports.Clear();
        IpOptions.Ports.AddRange(Ports.Where(p => p.IsChecked).Select(p => p.Port));
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
            saved = false;
        }
        if (MinimumCountOfValidIp > 0)
        {
            IpOptions.CountOfRepeatTestForEachIp = CountOfRepeatTestForEachIp;
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("error", "Minimum Count is 1", "ok");
            saved = false;

        }
        if (MaxPingOfIP > 100)
        {
            IpOptions.MaxPingOfIP = MaxPingOfIP;
        }
        else 
        {
            await App.Current.MainPage.DisplayAlert("error", "Minimum ping is 100", "ok");
            saved = false;

        }
        if (CountOfIpRanges > 0)
        {
            IpOptions.CountOfIpRanges = CountOfIpRanges;
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("error", "Minimum Count is 1", "ok");
            saved = false;

        }
        if (CountOfIpForTest > 0)
        {
            try
            {

            }
            catch
            {

            }
        }
        else
        {
            saved = false;
        }


        if (saved)
        {
            totalTestForEachIp = CountOfRepeatTestForEachIp * (Ports.Where(p=>p.IsChecked)).Count();
            var countOfIpInEachRange = CountOfIpForTest / CountOfIpRanges;

            await App.Current.MainPage.DisplayAlert("Info", $"{CountOfIpForTest} ips in {CountOfIpRanges} ranges will be tested {totalTestForEachIp} times", "ok");
            await Shell.Current.GoToAsync("../");
        }
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

    public void UpdateVar()
    {
        totalTestForEachIp = CountOfRepeatTestForEachIp * (Ports.Where(p => p.IsChecked)).Count();
    }
}
