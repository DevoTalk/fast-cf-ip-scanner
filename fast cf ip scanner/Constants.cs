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
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DatabaseFilename);
    }
}
