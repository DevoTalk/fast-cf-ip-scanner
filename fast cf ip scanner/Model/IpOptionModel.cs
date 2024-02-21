using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fast_cf_ip_scanner.Model
{
    public class IpOptionModel
    {
        public List<string> Ports { get; set; } = Constants.HttpPorts.Concat(Constants.HttpsPorts).ToList();

        public string MaxPingOfIP { get; set; } = "1000";

        public int MinimumCountOfValidIp { get; set; } = 5;
        public int CountOfRepeatTestForEachIp { get; set; } = 3;
    }
}
