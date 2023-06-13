using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fast_cf_ip_scanner.Model
{
    public class ConfigProviderModel
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool random { get; set; }
        public List<string> urls { get; set; }

    }
}
