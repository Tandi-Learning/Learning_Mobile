namespace Yahtzee.Data;

public static class Constants
{

    /*
     * Open ADB command prompt
     * These are the commands that can be used to access the database
     * - list all the files
     *   adb shell run-as com.companyname.yahtzee ls files
     * - copy to local folder from emulator
     *   adb pull /data/user/0/com.companyname.yahtzee/files/yahtzee.db3
     * - remove the database
     *   adb shell rm -f /data/user/0/com.companyname.yahtzee/files/appsqlite.db3
    */

    public const string DatabaseFilename = "yahtzee.db3";

    public static string DatabaseFilePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath { get; set; } = $"Data Source={DatabaseFilePath}";
}