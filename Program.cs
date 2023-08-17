using System.Data.SQLite;
using static Utils;

string firstGpkgPath = Environment.GetEnvironmentVariable("GPKG_A") ?? throw new NullReferenceException("GPKG_A cannot be null");
string secondGpkgPath = Environment.GetEnvironmentVariable("GPKG_B") ?? throw new NullReferenceException("GPKG_B cannot be null");
SQLiteConnection sqlite_connA = EstablishConnection(firstGpkgPath);
SQLiteConnection sqlite_connB = EstablishConnection(secondGpkgPath);

try
{
    MergeData(sqlite_connA, sqlite_connB);
}
catch (Exception e)
{
    throw new Exception("Merge failed", e);
}
finally
{
    sqlite_connA.Close();
    sqlite_connB.Close();
}
