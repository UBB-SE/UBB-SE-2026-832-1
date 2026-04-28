using System;
using System.IO;

namespace ClassLibrary.Data;

/// <summary>
/// Utility class responsible for resolving local database paths.
/// </summary>
public static class DatabasePaths
{
    
    public static string GetConnectionString()
    { 
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "UBB-SE");

        Directory.CreateDirectory(folder);

        string dbPath = Path.Combine(folder, "UBB-SE.db");

        return $"Data Source={dbPath}";
    }
}