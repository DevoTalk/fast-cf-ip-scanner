

using fast_cf_ip_scanner.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;


namespace fast_cf_ip_scanner.Services
{
    public class IPService
    {
        private readonly FastCFIPScannerDatabase _db;
        public IPService(FastCFIPScannerDatabase db)
        {
            _db = db;
        }


        public async Task<List<IPModel>> GetIpValid(string[] ips, int maxPing, string protcol)
        {
            var validIps = new List<IPModel>();
            switch (protcol)
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
                    validIps = await GetValidIPWithTCPTest(ips, maxPing);
                    break;
            }
            return validIps;
        }

        public async Task<List<IPModel>> GetValidٌIPWithHttpTest(string[] ips, int maxPing)
        {

            var validIp = new List<IPModel>();


            var randomIps = GetRandomIp(ips);
            foreach (var ipAddresse in randomIps)
            {
                var t = new Task(async () =>
                {
                    var stopwatch = new Stopwatch();

                    try
                    {
                        var SocketsHandler = new SocketsHttpHandler();
                        var Client = new HttpClient(SocketsHandler)
                        {
                            Timeout = TimeSpan.FromSeconds(maxPing),
                        };
                        int ping = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            stopwatch.Start();


                            var result = await Client.GetAsync($"http://{ipAddresse}/__down");


                            stopwatch.Stop();
                            ping += Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                        }
                        ping = ping / 3;
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
            var randip = GetRandomIp(ips);
            foreach (var ipAddresse in randip)
            {
                var t = new Task(() =>
                {
                    var stopwatch = new Stopwatch();

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
            var randips = GetRandomIp(ips);
            foreach (var ipAddresse in randips)
            {
                var t = new Task(() =>
                {
                    var stopwatch = new Stopwatch();

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



        public List<string> GetRandomIp(string[] ips)
        {

            Random random = new Random();

            var randomIpRange = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                var randIp = ips[random.Next(ips.Length)].Split("/")[0].Split(".").ToList();
                randIp.Remove(randIp.Last());
                var strIp = string.Join(".", randIp);
                strIp = strIp + ".";

                randomIpRange.Add(strIp);
            }
            var randomIps = new List<string>();
            foreach (var iprange in randomIpRange)
            {
                for (int i = 0; i < 5; i++)
                {
                    randomIps.Add(iprange + random.Next(255));
                }
            }
            return randomIps;
        }

        public async Task<string[]> GetIps()
        {
            var IpAddresses = await _db.GetAllIPs();
            return IpAddresses;
        }

        public async Task AddValidIpToDb(IPModel ip)
        {
            await _db.AddIP(ip);
        }
        public async Task AddValidIpToDb(List<IPModel> ips)
        {
            foreach (var ip in ips)
            {
                await _db.AddIP(ip);
            }
        }






    }
}
