
using fast_cf_ip_scanner.Data;
using fast_cf_ip_scanner.Model;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Reflection;
using System.Resources;

namespace fast_cf_ip_scanner.Services
{
    public class IPService
    {
        private readonly FastCFIPScannerDatabase _db;
        public IPService(FastCFIPScannerDatabase db)
        {
            _db = db;
        }


        public async Task<List<IPModel>> GetIpValid(string[]ips,int maxPing)
        {
            SocketsHttpHandler SocketsHandler = new SocketsHttpHandler();
            HttpClient Client = new HttpClient(SocketsHandler)
            {
                Timeout = TimeSpan.FromSeconds(maxPing),
            };

            
            var validIp = new List<IPModel>();
            for (int i = 0; i < 20; i++)
            {
                var t = new Task(async () =>
                {
                    var stopwatch = new Stopwatch();
                    var ipAddresse = GetRandomIp(ips);
                    HttpResponseMessage result = new HttpResponseMessage();
                    try
                    {
                        stopwatch.Start();
                        result = await Client.GetAsync($"http://{ipAddresse}/__down");
                        stopwatch.Stop();
                        var ping = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                        lock (validIp)
                        {
                            validIp.Add(new IPModel
                            {
                                IP = ipAddresse,
                                Ping = ping,
                            });
                        }
                    }
                    catch
                    {
                        stopwatch.Reset();
                    }
                });
                t.Start();
            }
            for (int i = 0; i < maxPing / 100; i++)
            {
                await Task.Delay(100);
                if (validIp.Count >= 10)
                {
                    return validIp;
                }
            }
            return validIp;
        }

        public async Task<string[]> GetIps()
        {
            var IpAddresses = await _db.GetAllIPs();
            return IpAddresses;
        }
        string GetRandomIp(string[] ips)
        {
            Random random = new Random();
            return ips[random.Next(ips.Length)];
        }
        public async Task addValidIpToDb(IPModel ip)
        {
            await _db.AddIP(ip);
        }
        public async Task addValidIpToDb(List<IPModel> ips)
        {
            foreach (var ip in ips)
            {
                await _db.AddIP(ip);
            }
        }
    }
}
