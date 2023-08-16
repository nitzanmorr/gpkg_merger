using System.Data.SQLite;
using static Utils;

if (Environment.GetEnvironmentVariable("GPKG_A") == null || Environment.GetEnvironmentVariable("GPKG_B") == null)
{
    throw new Exception("Must set GPKG_A and GPKG_B");
}
string firstGpkgPath = Environment.GetEnvironmentVariable("GPKG_A");
string secondGpkgPath = Environment.GetEnvironmentVariable("GPKG_B");
SQLiteConnection sqlite_connA = CreateConnection(firstGpkgPath);
SQLiteConnection sqlite_connB = CreateConnection(secondGpkgPath);

MergeData(sqlite_connA, sqlite_connB);