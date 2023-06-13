using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fast_cf_ip_scanner.Model
{
    public class ConfigModel
    {
        public string protocol { get; set; }
        public string id { get; set; }
        public string? add { get; set; }
        public string port { get; set; } = "443";
        public string? ps { get; set; }
        public string net { get; set; } = "tcp";
        public string? host { get; set; }
        public string? path { get; set; }
        public string tls { get; set; } = "none";
        public string? sni { get; set; }
        public string? alpn { get; set; }
        public string? fp { get; set; }
    }
}
