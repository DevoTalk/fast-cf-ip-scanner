using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fast_cf_ip_scanner.Model;

public class IpOptionModel
{
    public IpOptionModel()
    {
        Ports = new List<string>();
        Ports.AddRange(new List<string>() { "80", "443", "8080" });
    }
    public List<string> Ports { get; set; } 

    public int MaxPingOfIP { get; set; } = 1000;

    public int MinimumCountOfValidIp { get; set; } = 5;
    public int CountOfRepeatTestForEachIp { get; set; } = 1;
    public int CountOfIpRanges { get; set; } = 4;
    public int CountOfIpForTest { get; set; } = 20;
    public int DownloadSizeForSpeedTest { get; set; } = 10;
}
