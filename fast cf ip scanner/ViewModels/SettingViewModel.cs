using System.Text.RegularExpressions;

namespace fast_cf_ip_scanner.ViewModels
{
    public partial class SettingViewModel : BaseViewModel
    {
        [ObservableProperty]
        string newWorkerUrl;

        [ObservableProperty]
        ObservableCollection<WorkerModel> workers;

        private readonly WorkerService _services;
        public SettingViewModel(WorkerService service)
        {
            _services = service;
            workers = new ObservableCollection<WorkerModel>();
            LoadWorkers();
            
        }
        public async Task LoadWorkers()
        {
            Workers.Clear();
            var ws = await _services.GetWorkers();
            foreach (var w in ws)
            {
                Workers.Add(w);
            } 
        }

        [RelayCommand]
        async Task DeleteWorker(WorkerModel worker)
        {
            await _services.DeleteWorker(worker);
            await LoadWorkers();
        }

        [RelayCommand]
        async void AddNewWorkerUrl()
        {

            if (await isValidWorkerUrl(NewWorkerUrl))
            {
                _services.AddWorker(NewWorkerUrl);
            }
            await LoadWorkers();
        }
         async Task<bool> isValidWorkerUrl(string workerUrl)
        {
            if (workerUrl == null)
            {
                await App.Current.MainPage.DisplayAlert("error", "worker URL is empity", "OK");
                return false;
            }

            string pattern = @"^https:\/\/([a-z0-9]+(-[a-z0-9]+)*\.)*workers\.dev\/?$";
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(workerUrl))
            {
                await App.Current.MainPage.DisplayAlert("error", "worker URL is not mach with pattern", "OK");
                return false;
            }
            return true;
        }
    }
}
