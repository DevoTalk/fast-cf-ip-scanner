
using fast_cf_ip_scanner.Model;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Reflection;

namespace fast_cf_ip_scanner.Services
{
    public class IPServices
    {
        public IPServices()
        {
        }
        public async Task<List<IPModel>> GetIpValid(int maxPing)
        {
            SocketsHttpHandler SocketsHandler = new SocketsHttpHandler();
            HttpClient Client = new HttpClient(SocketsHandler)
            {
                Timeout = TimeSpan.FromSeconds(maxPing),
            };
            var tasks = new Task[20];
            var validIp = new List<IPModel>();
            for (int i = 0; i < 20; i++)
            {
                var t = new Task(async () =>
                {
                    var stopwatch = new Stopwatch();
                    var ipAddresse = GetRandomIp();
                    HttpResponseMessage result = new HttpResponseMessage();
                    try
                    {
                        stopwatch.Start();
                        result = await Client.GetAsync($"http://{ipAddresse}");
                        stopwatch.Stop();


                        //var request = (HttpWebRequest)WebRequest.Create($"http://{ipAddresse}");
                        //request.Timeout = 5000;
                        //var response = (HttpWebResponse)request.GetResponse();
                        //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


                        var ping = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                        lock (validIp)
                        {
                            validIp.Add(new IPModel
                            {
                                IP = ipAddresse,
                                Ping = ping,
                                IsValid = true
                            });
                        }
                    }
                    catch
                    {
                        stopwatch.Reset();
                    }
                });
                tasks[i] = t;
                t.Start();
            }
            await Task.Delay(maxPing);
            //Thread.Sleep(5000);
            return validIp;
        }
        string GetRandomIp()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"IPAddresses.txt");
            var ipAddresses = File.ReadAllLines(path);
            Random random = new Random();
            return ipAddresses[random.Next(ipAddresses.Length)];
        }
    }
}
