
using fast_cf_ip_scanner.Data;
using fast_cf_ip_scanner.Model;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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


        public async Task<List<IPModel>> GetIpValid(string[] ips, int maxPing,string protcol)
        {
            var validIps = new List<IPModel>();
            switch(protcol)
            {

                case "Http test":
                    validIps = await GetValidٌIPWithHttpTest(ips, maxPing);
                    break;

                case "TCP test":
                    validIps = await GetValidIPWithTCPTest(ips, maxPing);
                    break;

                case "UDP test":
                    validIps = await GetValidIPWithUDPTest(ips, maxPing);
                    break;

                default:
                    validIps = await GetValidIPWithTCPTest(ips,maxPing);
                    break;
            }
            return validIps;
        }

        public async Task<List<IPModel>> GetValidٌIPWithHttpTest(string[] ips, int maxPing)
        {
            
            var validIp = new List<IPModel>();
            for (int i = 0; i < 20; i++)
            {
                var t = new Task(async () =>
                {
                    var stopwatch = new Stopwatch();
                    var ipAddresse = IPAddress.Parse(GetRandomIp(ips).Split("/")[0]);

                    try
                    {
                        var SocketsHandler = new SocketsHttpHandler();
                        var Client = new HttpClient(SocketsHandler)
                        {
                            Timeout = TimeSpan.FromSeconds(maxPing),
                        };


                        stopwatch.Start();


                        var result = await Client.GetAsync($"http://{ipAddresse}/__down");


                        stopwatch.Stop();
                        var ping = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                        lock (validIp)
                        {
                            validIp.Add(new IPModel
                            {
                                IP = ipAddresse.ToString(),
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
            for (int i = 0; i < maxPing * 2 / 100; i++)
            {
                await Task.Delay(100);
                if (validIp.Count >= 10)
                {
                    return validIp;
                }
            }
            return validIp;
        }


        public async Task<List<IPModel>> GetValidIPWithTCPTest(string[] ips, int maxPing)
        {

            var validIp = new List<IPModel>();
            for (int i = 0; i < 20; i++)
            {
                var t = new Task(() =>
                {
                    var stopwatch = new Stopwatch();
                    var ipAddresse = IPAddress.Parse(GetRandomIp(ips).Split("/")[0]);
                    try
                    {
                        var client = new TcpClient();

                        stopwatch.Start();


                        client.Connect(ipAddresse, 443);


                        stopwatch.Stop();

                        var ping = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                        lock (validIp)
                        {
                            validIp.Add(new IPModel
                            {
                                IP = ipAddresse.ToString(),
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
            for (int i = 0; i < maxPing * 2 / 100; i++)
            {
                await Task.Delay(100);
                if (validIp.Count >= 10)
                {
                    return validIp;
                }
            }
            return validIp;
        }

        public async Task<List<IPModel>> GetValidIPWithUDPTest(string[] ips, int maxPing)
        {

            var validIp = new List<IPModel>();
            for (int i = 0; i < 20; i++)
            {
                var t = new Task(() =>
                {
                    var stopwatch = new Stopwatch();
                    var ipAddresse = IPAddress.Parse(GetRandomIp(ips).Split("/")[0]);
                    
                    try
                    {
                        var client = new UdpClient();

                        stopwatch.Start();


                        client.Connect(ipAddresse, 443);


                        stopwatch.Stop();

                        var ping = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                        lock (validIp)
                        {
                            validIp.Add(new IPModel
                            {
                                IP = ipAddresse.ToString(),
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
            for (int i = 0; i < maxPing * 2 / 100; i++)
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
