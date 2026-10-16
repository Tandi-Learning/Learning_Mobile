using SQLite;

namespace Yahtzee.Data
{
    public static class SqliteService
    {
        private static SQLiteAsyncConnection? _database;

        // Call this early in app startup (MainPage or MauiProgram)
        public static void Init(string? databasePath = null)
        {
            // Initialize native SQLite provider
            try
            {
                SQLitePCL.Batteries_V2.Init();
            }
            catch
            {
                // ignore if already initialized
            }

            if (!string.IsNullOrEmpty(databasePath))
            {
                Constants.DatabasePath = databasePath!;
            }

            if (string.IsNullOrEmpty(Constants.DatabasePath))
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                Constants.DatabasePath = Path.Combine(folder, Constants.DatabaseFilename);
            }

            _database = new SQLiteAsyncConnection(Constants.DatabasePath);
        }

        public static SQLiteAsyncConnection GetConnection()
        {
            if (_database is null)
            {
                Init();
            }
            return _database!;
        }
    }
}
