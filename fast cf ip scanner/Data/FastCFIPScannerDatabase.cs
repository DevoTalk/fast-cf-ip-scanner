using Microsoft.Extensions.Caching.Memory;
using SQLite;
namespace fast_cf_ip_scanner.Data
{

    public class FastCFIPScannerDatabase
    {
        SQLiteAsyncConnection DataBase;
        MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        async Task Init()
        {
            if (DataBase is not null)
                return;
            DataBase = new SQLiteAsyncConnection(Constants.GetDatabasePath(), Constants.Flags);
            await DataBase.CreateTableAsync<IPModel>();
            await DataBase.CreateTableAsync<WorkerModel>();

        }
        public async Task<string[]> GetAllIPs()
        {
            if (Cache.TryGetValue("AllIPs", out string[] ipList))
            {
                return ipList;
            }

            string[] urls = new string[] { "https://getip.ali1707.workers.dev", "https://raw.githubusercontent.com/Ali1707/cloudflare-ips/main/ips.txt" };

            HttpClient client = new HttpClient();

            var tasks = new Task<string>[urls.Length];

            try
            {
                for (int i = 0; i < urls.Length; i++)
                {
                    tasks[i] = client.GetStringAsync(urls[i]);
                }
                var completedTask = await Task.WhenAny(tasks);
                var stringIP = completedTask.Result;

                var ips = stringIP.Split(",");

                Cache.Set("AllIPs", ips);

                return ips;
            }
            catch (Exception ex)
            {
                return new string[0];
            }
        }

        public async Task AddIP(IPModel ip)
        {
            await Init();
            await DataBase.InsertAsync(ip);
        }
        public async Task AddWorker(WorkerModel workerUrl)
        {
            await Init();
            await DataBase.InsertAsync(workerUrl);
        }
        public async Task<List<WorkerModel>> GetAllWorker()
        {
            await Init();
            return await DataBase.Table<WorkerModel>().ToListAsync();
        }
        public async Task DeleteWorker(WorkerModel worker)
        {
            await Init();
            await DataBase.Table<WorkerModel>().Where(w => w.Url == worker.Url).DeleteAsync();
        }
    }
}
