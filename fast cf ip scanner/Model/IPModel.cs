using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fast_cf_ip_scanner.Model
{
    public class IPModel
    {
        public string IP { get; set; }
        public int Ping { get; set; }
        public bool IsValid { get; set; }
    }
}
