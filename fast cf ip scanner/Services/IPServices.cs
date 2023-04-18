
using fast_cf_ip_scanner.Model;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Reflection;

namespace fast_cf_ip_scanner.Services
{
    public class IPServices
    {
        SocketsHttpHandler SocketsHandler;
        HttpClient Client;
        public IPServices()
        {
            SocketsHandler = new SocketsHttpHandler();
            Client = new HttpClient(SocketsHandler) 
            {
                Timeout = TimeSpan.FromSeconds(5),
            };
        }
        public List<IPModel> GetIpValid()
        {
            var stopwatch = new Stopwatch();
            var validIp = new List<IPModel>();
            for (int i = 0; i < 20; i++)
            {
                #region Send request
                var t = new Task(async () =>
                {
                    var ipAddresse = GetRandomIp();
                    try
                    {
                        stopwatch.Start();
                        var result = await Client.GetAsync($"http://{ipAddresse}/");
                        stopwatch.Stop();
                        var ping = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                        validIp.Add(new IPModel
                        {
                            IP = ipAddresse,
                            Ping = ping,
                            IsValid = true
                        });
                    }
                    catch
                    {
                        stopwatch.Reset();
                    }
                });
                t.Start();
                #endregion
            }

            Thread.Sleep(5000);
            validIp.OrderByDescending(v => v.Ping);
            return validIp;
        }
        public string GetRandomIp()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"IPAddresses.txt");
            var ipAddresses = File.ReadAllLines(path);
            Random random = new Random();
            return ipAddresses[random.Next(ipAddresses.Length)];
            
        }
    }
}
