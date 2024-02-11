

using fast_cf_ip_scanner.Data;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
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

        #region Test Ips

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
                        int totalPing = 0;
                        var randomPorts = GetRandomPort();
                        foreach (var port in randomPorts)
                        {
                            stopwatch.Start();


                            var result = await Client.GetAsync($"http://{ipAddresse}/__down:{port}");


                            stopwatch.Stop();
                            var currentPing = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                            totalPing += currentPing;
                        }
                        int ping = totalPing / 3;
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
                if (validIp.Count >= 5)
                {
                    return validIp;
                }
            }
            return validIp;
        }

        public async Task<List<IPModel>> GetValidIPWithTCPTest(string[] ips, int maxPing)
        {

            var validIp = new List<IPModel>();

            async Task TestConnectionAsync(string ip)
            {
                TcpClient tcpClient = new TcpClient();
                var randomPort = GetRandomPort(1);
                try
                {
                    tcpClient.ConnectAsync(ip, int.Parse(randomPort.First()));
                    await Task.Delay(maxPing);
                    if (tcpClient.Connected)
                    {
                        lock (validIp)
                        {
                            validIp.Add(new IPModel
                            {
                                IP = ip,
                                Ping = 0,
                            });
                        }
                    }
                }
                catch
                {

                }
            }
            while (true)
            {
                if (validIp.Count >= 5)
                {
                    break;
                }
                var newRandomIps = GetRandomIp(ips);

                List<Task> tasks = new List<Task>();

                foreach (var ip in newRandomIps)
                {
                    tasks.Add(TestConnectionAsync(ip));
                }
                await Task.Delay(maxPing + 100);
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
                if (validIp.Count >= 5)
                {
                    return validIp;
                }
            }
            return validIp;
        }
        #endregion


        public List<string> GetRandomIp(string[] ips, int ipCount = 20, int ipRangeCount = 4)
        {

            Random random = new Random();

            var randomIpRange = new List<string>();
            for (int i = 0; i < ipRangeCount; i++)
            {
                var randIp = ips[random.Next(ips.Length)].Split("/")[0].Split(".").ToList();

                randIp.Remove(randIp.Last());

                var strIp = string.Join(".", randIp);

                strIp = strIp + ".";

                randomIpRange.Add(strIp);
            }
            var randomIps = new List<string>();
            var countOfEachRange = (ipCount / ipRangeCount) + 1;
            foreach (var iprange in randomIpRange)
            {
                for (int i = 1; i < countOfEachRange; i++)
                {
                    randomIps.Add(iprange + random.Next(255));
                }
            }
            return randomIps;
        }
        public List<string> GetRandomPort(int count = 3)
        {
            Random random = new Random();
            var randomPorts = new List<string>();
            var allPorts = Constants.HttpPorts.Concat(Constants.HttpsPorts).ToList();

            for (int i = 0; i < count; i++)
            {
                int randomIndex = random.Next(allPorts.Count);
                randomPorts.Add(allPorts[randomIndex]);
            }
            return randomPorts;
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
