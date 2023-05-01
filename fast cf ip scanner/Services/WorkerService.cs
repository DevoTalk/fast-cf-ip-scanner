using fast_cf_ip_scanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fast_cf_ip_scanner.Services
{
    public class WorkerService
    {
        private readonly FastCFIPScannerDatabase _db;
        public WorkerService(FastCFIPScannerDatabase db)
        {
            _db = db;
        }
        public async void AddWorker(string WorkerUrl)
        {
            await _db.AddWorker(new WorkerModel { Url = WorkerUrl });
        }
        public async Task<List<WorkerModel>> GetWorkers()
        {
            return await _db.GetAllWorker();
        }
        public async Task DeleteWorker(WorkerModel worker)
        {
           await _db.DeleteWorker(worker);
        }
        public async Task<bool> isValidWorkerUrl(string workerUrl)
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
