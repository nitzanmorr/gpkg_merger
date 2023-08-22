using System.Data.SQLite;
using static Utils;

string firstGpkgPath = Environment.GetEnvironmentVariable("GPKG_A") ?? throw new NullReferenceException("GPKG_A cannot be null");
string secondGpkgPath = Environment.GetEnvironmentVariable("GPKG_B") ?? throw new NullReferenceException("GPKG_B cannot be null");
SQLiteConnection sqliteConnA = EstablishConnection(firstGpkgPath);
SQLiteConnection sqliteConnB = EstablishConnection(secondGpkgPath);

try
{
    MergeData(sqliteConnA, sqliteConnB);
}
catch (Exception e)
{
    throw new Exception("Merge failed", e);
}
finally
{
    sqliteConnA.Close();
    sqliteConnB.Close();
}
