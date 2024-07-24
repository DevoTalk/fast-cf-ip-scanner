using fast_cf_ip_scanner.Data;
using Microsoft.Maui.Animations;
using System.Collections.Concurrent;
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
        public async Task<List<IPModel>> GetIpValid(string[] ips, IpOptionModel ipOptions, string protcol)
        {
            var validIps = new List<IPModel>();
            switch (protcol)
            {
                case "Http test (recommended)":
                    validIps = await GetValidIPWithHttpTest(ips, ipOptions);
                    break;

                case "TCP test":
                    validIps = await GetValidIPWithTCPTest(ips, ipOptions);
                    break;

                //case "UDP test":
                //    validIps = await GetValidIPWithUDPTest(ips, ipOptions);
                //    break;
                
                case "Terminal Ping test":
                    validIps = await GetValidIpWithPingTest(ips, ipOptions);
                    break;

                default:
                    validIps = await GetValidIPWithHttpTest(ips, ipOptions);
                    break;
            }
            return validIps;
        }

        public async Task<List<IPModel>> GetValidIPWithHttpTest(string[] ips, IpOptionModel ipOptions)
        {
            var validIp = new ConcurrentBag<IPModel>();

            async Task HttpTest(string ipAddresse)
            {
                var stopwatch = new Stopwatch();

                var SocketsHandler = new SocketsHttpHandler();
                var Client = new HttpClient(SocketsHandler)
                {
                    Timeout = TimeSpan.FromSeconds(ipOptions.MaxPingOfIP),
                };

                var ports = new List<string>();

                var timeoutCount = 0;
                for (int i = 0; i < ipOptions.CountOfRepeatTestForEachIp; i++)
                {
                    foreach (var port in ipOptions.Ports)
                    {
                        try
                        {
                            stopwatch.Start();
                            var result = await Client.GetAsync($"http://{ipAddresse}/__down:{port}");
                            stopwatch.Stop();

                            if (result != null)
                            {
                                if (result.Headers.Contains("Server") && result.Headers.GetValues("Server").Contains("cloudflare"))
                                {
                                    if (!ports.Any(p => p == port))
                                    {
                                        ports.Add(port);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            timeoutCount++;
                        }
                    }
                }
                if (ports.Count > 0)
                {
                    var totalPing = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                    int ping = totalPing / (ipOptions.CountOfRepeatTestForEachIp * ipOptions.Ports.Count);

                    validIp.Add(new IPModel
                    {
                        IP = ipAddresse.ToString(),
                        Ping = ping,
                        Ports = string.Join(",", ports),
                        CountOfTimeout = timeoutCount
                    });
                }
            }
            #region repeat test
            while (true)
            {
                if (validIp.Count >= ipOptions.MinimumCountOfValidIp)
                {
                    return validIp.ToList();
                }
                var newRandomIps = GetRandomIp(ips, ipOptions.CountOfIpForTest, ipOptions.CountOfIpRanges);

                List<Task> tasks = new List<Task>();

                foreach (var ip in newRandomIps)
                {
                    tasks.Add(HttpTest(ip));
                }
                for (int i = 0; i < ipOptions.MaxPingOfIP / 100; i++)
                {
                    await Task.Delay(100);
                    if (validIp.Count >= ipOptions.MinimumCountOfValidIp)
                    {
                        return validIp.ToList();
                    }
                }
            }
            #endregion
        }

        public async Task<List<IPModel>> GetValidIPWithTCPTest(string[] ips, IpOptionModel ipOptions)
        {
            var validIp = new ConcurrentBag<IPModel>();

            async Task TestConnectionAsync(string ip)
            {
                var ports = new List<string>();
                var stopwatch = new Stopwatch();
                var totolTimeOut = 0;
                var totalPing = 0;
                for (int i = 0; i < ipOptions.CountOfRepeatTestForEachIp; i++)
                {
                    foreach (var port in ipOptions.Ports)
                    {
                        try
                        {
                            using (var tcpClient = new TcpClient())
                            {
                                stopwatch.Restart();
                                var connectTask = tcpClient.ConnectAsync(ip, int.Parse(port));
                                // Wait for either connection or timeout
                                await Task.WhenAny(connectTask, Task.Delay(ipOptions.MaxPingOfIP));

                                if (tcpClient.Connected)
                                {
                                    stopwatch.Stop();
                                    totalPing += Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                                    if (!ports.Any(p => p == port))
                                    {
                                        ports.Add(port);
                                    }
                                }
                                else
                                {
                                    totolTimeOut++;
                                }
                            }
                        }
                        catch
                        {
                            totolTimeOut++;
                        }
                    }
                }

                if (ports.Count > 0)
                {
                    int ping = totalPing / (ipOptions.CountOfRepeatTestForEachIp * ipOptions.Ports.Count);

                    validIp.Add(new IPModel
                    {
                        IP = ip,
                        Ping = ping,
                        Ports = string.Join(",", ports),
                        CountOfTimeout = totolTimeOut
                    });
                }
            }

            while (validIp.Count < ipOptions.MinimumCountOfValidIp)
            {
                var newRandomIps = GetRandomIp(ips, ipOptions.CountOfIpForTest, ipOptions.CountOfIpRanges);
                var tasks = new List<Task>();

                foreach (var ip in newRandomIps)
                {
                    tasks.Add(TestConnectionAsync(ip));
                }

                for (int i = 0; i < ipOptions.MaxPingOfIP / 100; i++)
                {
                    await Task.Delay(100);
                    if (validIp.Count >= ipOptions.MinimumCountOfValidIp)
                    {
                        return validIp.ToList();
                    }
                }
            }

            return validIp.ToList();
        }
        public async Task<List<IPModel>> GetValidIPWithUDPTest(string[] ips, int maxPing)
        {
            var validIp = new ConcurrentBag<IPModel>();
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
                    return validIp.ToList();
                }
            }
            return validIp.ToList();
        }
        public async Task<List<IPModel>> GetValidIpWithPingTest(string[] ips, IpOptionModel ipOptions)
        {
            var validIp = new ConcurrentBag<IPModel>();

            async Task TestConnectionAsync(string ipAddress)
            {
                var ping = 0;
                var ipIsConnected = false;
                var totalTimeOut = 0;
                for (int i = 0; i < ipOptions.CountOfRepeatTestForEachIp; i++)
                {
                    using (Ping pingSender = new Ping())
                    {
                        try
                        {
                            var reply = await pingSender.SendPingAsync(ipAddress, ipOptions.MaxPingOfIP);
                            if (reply.Status == IPStatus.Success)
                            {
                                ipIsConnected = true;
                                ping += (int)reply.RoundtripTime;
                            }
                            else
                            {
                                totalTimeOut += 1;
                            }
                        }
                        catch
                        {
                            totalTimeOut += 1;
                        }
                    }
                }
                if (ipIsConnected)
                {
                    var avgPing = ping / ipOptions.CountOfRepeatTestForEachIp;

                    validIp.Add(new()
                    {
                        IP = ipAddress,
                        Ping = avgPing,
                        CountOfTimeout = totalTimeOut
                    });
                }
            }

            while (validIp.Count < ipOptions.MinimumCountOfValidIp)
            {
                var newRandomIps = GetRandomIp(ips, ipOptions.CountOfIpForTest, ipOptions.CountOfIpRanges);
                var tasks = new List<Task>();

                foreach (var ip in newRandomIps)
                {
                    tasks.Add(TestConnectionAsync(ip));
                }
                for (int i = 0; i < ipOptions.MaxPingOfIP / 100; i++)
                {
                    await Task.Delay(100);
                    if (validIp.Count >= ipOptions.MinimumCountOfValidIp)
                    {
                        return validIp.ToList();
                    }
                }
            }
            return validIp.ToList();
        }
        public async Task<double> GetDownloadSpeedAsync(string testUrl, string ipAddress)
        {
            // Replace the host in the test URL with the target IP address.
            var uriBuilder = new UriBuilder(testUrl)
            {
                Host = ipAddress
            };
            var targetUrl = uriBuilder.Uri;

            using (var httpClient = new HttpClient())
            {
                // Set the 'Host' header to the original host name.
                httpClient.DefaultRequestHeaders.Host = new Uri(testUrl).Host;

                // Measure the time taken to download the file.
                var stopwatch = Stopwatch.StartNew();
                var response = await httpClient.GetAsync(targetUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // Read the content as byte array to ensure the download is completed.
                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                stopwatch.Stop();       
                // Calculate download speed in Mbps (Mega bits per second).
                double fileSizeInMegabytes = contentBytes.Length / (1024d * 1024d);
                double durationInSeconds = stopwatch.Elapsed.TotalSeconds;
                double speedMBps = fileSizeInMegabytes / durationInSeconds; // Convert MBps to Mbps
                var speedMbps = speedMBps * 8;
                return Math.Round(speedMbps, 2);
            }
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
            var countOfEachRange = ipCount / ipRangeCount;
            foreach (var iprange in randomIpRange)
            {
                for (int i = 1; i < countOfEachRange; i++)
                {
                    randomIps.Add(iprange + random.Next(255));
                }
            }
            return randomIps;
        }
        public string[] GetRandomPort(int count = 3)
        {
            Random random = new Random();
            string[] randomPorts = new string[count];
            var allPorts = Constants.HttpPorts.Concat(Constants.HttpsPorts).ToList();

            for (int i = 0; i < count; i++)
            {
                int randomIndex = random.Next(allPorts.Count);
                randomPorts[i] = allPorts[randomIndex];
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
