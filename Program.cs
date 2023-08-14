using System.Data.SQLite;
using System.Linq;

String Area1GpkgPath = "/home/nitzanm5/Downloads/area1.gpkg";
// String Area3GpkgPath = "/home/nitzanm5/Downloads/area3.gpkg";

SQLiteConnection sqlite_conn = CreateConnection(Area1GpkgPath);
ReadData(sqlite_conn);


static SQLiteConnection CreateConnection(String sqlitePath)
// create a connection to a sqlite db
{
    SQLiteConnection sqlite_conn = new SQLiteConnection($"Data Source={sqlitePath}");
    try
    {
        sqlite_conn.Open();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    return sqlite_conn;
}

static void ReadData(SQLiteConnection conn)
{
    SQLiteDataReader sqlite_datareader;
    SQLiteCommand sqlite_cmd;
    sqlite_cmd = conn.CreateCommand();
    sqlite_cmd.CommandText = "SELECT * FROM O_arzi_mz_w84geo_Apr19_gpkg_18_0";

    sqlite_datareader = sqlite_cmd.ExecuteReader();
    while (sqlite_datareader.Read())
    {
        var myreader = sqlite_datareader.GetValue(4);
        var name = sqlite_datareader.GetName(4);
        Console.WriteLine($"{name} {myreader}");
    }
    conn.Close();
}
