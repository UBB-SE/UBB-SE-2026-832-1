using System;
using System.IO;

namespace ClassLibrary.Data;

public static class DatabasePaths
{
    private const string APPLICATION_DIRECTORY_NAME = "UBB-SE";
    private const string DATABASE_FILE_NAME = "UBB-SE.db";

    public static string GetConnectionString()
    {
        string applicationDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            APPLICATION_DIRECTORY_NAME);

        Directory.CreateDirectory(applicationDataDirectory);

        string databasePath = Path.Combine(applicationDataDirectory, DATABASE_FILE_NAME);

        return $"Data Source={databasePath}";
    }
}