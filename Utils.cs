using System.Data.SQLite;

class Utils
{
    public static SQLiteConnection EstablishConnection(String sqlitePath)
    // create a connection to a sqlite db
    {
        SQLiteConnection sqliteConn = new($"Data Source={sqlitePath}");
        try
        {
            sqliteConn.Open();
            return sqliteConn;
        }
        catch (Exception e)
        {
            throw new Exception("Cannot open the sqlite connection", e);
        }
    }

    public static void MergeData(SQLiteConnection connA, SQLiteConnection connB)
    {
        SQLiteDataReader sqliteDatareader;

        SQLiteCommand getBContent = connB.CreateCommand();
        getBContent.CommandText = "SELECT table_name FROM gpkg_contents LIMIT 1";
        sqliteDatareader = getBContent.ExecuteReader();
        sqliteDatareader.Read();
        string tableNameB = sqliteDatareader.GetString(Consts.FirstGpkgColumn);

        SQLiteCommand getAContent = connA.CreateCommand();
        getAContent.CommandText = "SELECT table_name FROM gpkg_contents LIMIT 1";
        sqliteDatareader = getAContent.ExecuteReader();
        sqliteDatareader.Read();
        string tableNameA = sqliteDatareader.GetString(Consts.FirstGpkgColumn);

        SQLiteCommand query = connA.CreateCommand();
        string secondGpkgAlias = "gpkgB";
        query.CommandText =
        $@"ATTACH DATABASE '{connB.FileName}' AS {secondGpkgAlias};
        INSERT OR IGNORE INTO {tableNameA} (zoom_level, tile_column, tile_row, tile_data)
        SELECT zoom_level, tile_column, tile_row, tile_data
        FROM gpkgB.{tableNameB};";

        try
        {
            query.ExecuteNonQuery();
            SetNewExtent(connA, secondGpkgAlias);
            Console.WriteLine("gpkg's merged successfully");
        }
        catch
        {
            throw;
        }


    }

    private static void SetNewExtent(SQLiteConnection connA, string gpkgBAlias)
    {
        SQLiteCommand setMergedExtentsQuery = new SQLiteCommand(
            @$"UPDATE gpkg_contents
            SET min_x = MIN(min_x, (SELECT min_x FROM {gpkgBAlias}.gpkg_contents)),
                min_y = MIN(min_y, (SELECT min_y FROM {gpkgBAlias}.gpkg_contents)),
                max_x = MAX(max_x, (SELECT max_x FROM {gpkgBAlias}.gpkg_contents)),
                max_y = MAX(max_y, (SELECT max_y FROM {gpkgBAlias}.gpkg_contents))"
        , connA);

        try
        {
            setMergedExtentsQuery.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
    }
}