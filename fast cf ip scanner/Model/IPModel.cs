using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace fast_cf_ip_scanner.Model
{
    public class IPModel
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        
        public string IP { get; set; }
        
        public int Ping { get; set; }
    }
}
