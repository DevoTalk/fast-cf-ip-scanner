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
        //#if WINDOWS
        //        public static string DatabasePath =>
        //            Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"fast cf ip scanner", DatabaseFilename);
        //#endif
        //#if ANDROID
        //        public static string DatabasePath =>
        //            Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);
        //#endif
        public static string GetDatabasePath()
        {
#if WINDOWS
        string platformFolder = "Fast-CF-IP-Scanner";
#else
            string platformFolder = ""; // Adjust if needed for Android

#endif

            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), platformFolder);
            string databasePath = Path.Combine(appDataPath, DatabaseFilename);

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            return databasePath;
        }
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
