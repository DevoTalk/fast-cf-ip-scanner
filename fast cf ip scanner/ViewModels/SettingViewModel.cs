﻿using System.Text.RegularExpressions;

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

            if (await _services.isValidWorkerUrl(NewWorkerUrl))
            {
                _services.AddWorker(NewWorkerUrl);
            }
            await LoadWorkers();
        }
    }
}
