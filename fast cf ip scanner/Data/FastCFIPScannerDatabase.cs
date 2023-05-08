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

            DataBase = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await DataBase.CreateTableAsync<IPModel>();
            await DataBase.CreateTableAsync<WorkerModel>();
            
        }
        public async Task<string[]> GetAllIPs()
        {
            if (Cache.TryGetValue("AllIPs", out string[] ipList))
            {
                return ipList;
            }
            HttpClient client = new HttpClient();
            
            try
            {
                var stringIP = await client.GetStringAsync("https://getip.ali1707.workers.dev");

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
