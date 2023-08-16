using System.Data.SQLite;
using System.Security.Cryptography.X509Certificates;

class Utils
{
    public static SQLiteConnection CreateConnection(String sqlitePath)
    // create a connection to a sqlite db
    {
        SQLiteConnection sqlite_conn = new($"Data Source={sqlitePath}");
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
    public static void MergeData(SQLiteConnection connA, SQLiteConnection connB)
    {
        SQLiteDataReader sqlite_datareader;

        SQLiteCommand getBContent = connB.CreateCommand();
        getBContent.CommandText = "SELECT table_name FROM gpkg_contents LIMIT 1";
        sqlite_datareader = getBContent.ExecuteReader();
        sqlite_datareader.Read();
        string tableNameB = sqlite_datareader.GetString(0);

        SQLiteCommand getAContent = connA.CreateCommand();
        getAContent.CommandText = "SELECT table_name FROM gpkg_contents LIMIT 1";
        sqlite_datareader = getAContent.ExecuteReader();
        sqlite_datareader.Read();
        string tableNameA = sqlite_datareader.GetString(0);

        SQLiteCommand query = connA.CreateCommand();
        query.CommandText = $@"ATTACH DATABASE '{connB.FileName}' AS gpkgB;
        INSERT OR IGNORE INTO {tableNameA} 
        SELECT NULL, zoom_level, tile_column, tile_row, tile_data
        FROM gpkgB.{tableNameB};";

        Console.WriteLine(query.CommandText);

        try
        {
            query.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        SetNewExtent(connA);

        Console.WriteLine("gpkg's merged successfully");

        connA.Close();
        connB.Close();
    }

    private static void SetNewExtent(SQLiteConnection connA)
    {
        SQLiteCommand getExtents = new SQLiteCommand(
            @$"UPDATE gpkg_contents 
            SET min_x = MIN(min_x, (SELECT min_x FROM gpkgB.gpkg_contents)),
                min_y = MIN(min_y, (SELECT min_y FROM gpkgB.gpkg_contents)),
                max_x = MAX(max_x, (SELECT max_x FROM gpkgB.gpkg_contents)),
                max_y = MAX(max_y, (SELECT max_y FROM gpkgB.gpkg_contents))"
        , connA);

        try
        {
            getExtents.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}