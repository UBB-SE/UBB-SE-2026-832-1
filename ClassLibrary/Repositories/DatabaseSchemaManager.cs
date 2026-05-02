using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// EF Core based implementation of <see cref="IDatabaseSchemaManager"/>.
    ///
    /// This performs database creation via EF Core's EnsureCreated and then
    /// runs a very small compatibility migration to add the `type` column to
    /// the legacy `WORKOUT_LOG` table if the column does not exist. The
    /// original external source performed many DDL statements directly; to
    /// keep repository responsibilities small we've removed non-repository
    /// logic and only kept schema-initialization here.
    /// </summary>
    public sealed class DatabaseSchemaManager : IDatabaseSchemaManager
    {
        private readonly AppDbContext dbContext;

        public DatabaseSchemaManager(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void EnsureSchemaCreated()
        {
            // Ensure EF Core creates the schema for the application's model.
            // This keeps data access via EF Core and avoids maintaining raw SQL
            // creation scripts inside the repo.
            this.dbContext.Database.EnsureCreated();

            // Open a DB connection to perform a tiny compatibility migration
            // that was present in the original external source. We use the
            // underlying ADO connection exposed by EF Core to run the DDL if
            // required.
            var connection = (DbConnection)this.dbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            // Check if the WORKOUT_LOG table already has a `type` column. If
            // not, add it as TEXT NOT NULL with default 'CUSTOM'.
            using var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = @"SELECT 1 FROM pragma_table_info('WORKOUT_LOG') WHERE name = 'type' LIMIT 1;";
            var exists = checkCmd.ExecuteScalar() is not null;
            if (!exists)
            {
                using var alterCmd = connection.CreateCommand();
                alterCmd.CommandText = @"ALTER TABLE WORKOUT_LOG ADD COLUMN type TEXT NOT NULL DEFAULT 'CUSTOM';";
                alterCmd.ExecuteNonQuery();
            }
        }
    }
}
