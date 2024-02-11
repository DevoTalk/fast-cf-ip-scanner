using Microsoft.Maui.Storage;
using System.Xml.Linq;
namespace fast_cf_ip_scanner
{
    public static class Constants
    {
        public const string DatabaseFilename = "FastCFIPScanner.db";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;
        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static List<string> HttpPorts
        {
            get
            {
                return new List<string> {
                    "80",
                    "8080",
                    "8880",
                    "2052",
                    "2082",
                    "2086",
                    "2095"
                };
            }
        }
        public static List<string> HttpsPorts
        {
            get
            {
                return new List<string> {
                    "443",
                    "2053",
                    "2083",
                    "2087",
                    "2096",
                    "8443",
                };
            }
        }
    }
}
