
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

        
        public async Task<List<IPModel>> GetIpValid(int maxPing)
        {
            SocketsHttpHandler SocketsHandler = new SocketsHttpHandler();
            HttpClient Client = new HttpClient(SocketsHandler)
            {
                Timeout = TimeSpan.FromSeconds(maxPing),
            };

            var IpAddresses = await _db.GetAllIPs();
            var validIp = new List<IPModel>();
            for (int i = 0; i < 20; i++)
            {
                var t = new Task(async () =>
                {
                    var stopwatch = new Stopwatch();
                    var ipAddresse = GetRandomIp(IpAddresses);
                    HttpResponseMessage result = new HttpResponseMessage();
                    try
                    {
                        stopwatch.Start();
                        result = await Client.GetAsync($"http://{ipAddresse}");
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
            for (int i = 0;i < maxPing/100; i++)
            {
                await Task.Delay(100);
                if (validIp.Count >= 10)
                {
                    return validIp;
                }
            }
            return validIp;
        }
        string GetRandomIp(List<IPModel> ips)
        {
            Random random = new Random();
            return ips[random.Next(ips.Count)].IP;
        }
    }
}
