namespace ClassLibrary.IRepositories
{
    /// <summary>
    /// Responsible for ensuring the EF Core database schema is created and
    /// applying minimal compatibility migrations for older non-EF schemas.
    ///
    /// Note: any non-repository logic from the original external source has
    /// been intentionally removed. This service performs only schema creation
    /// and lightweight DB migrations.
    /// </summary>
    public interface IDatabaseSchemaManager
    {
        /// <summary>
        /// Ensures that the database schema exists and applies compatibility
        /// migrations if required.
        /// </summary>
        void EnsureSchemaCreated();
    }
}
