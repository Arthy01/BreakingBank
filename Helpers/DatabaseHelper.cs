using Npgsql;

namespace BreakingBank.Helpers
{
    public class DatabaseHelper : IDisposable
    {
        private readonly NpgsqlDataSource _dataSource;
        private bool _disposed = false;

        public DatabaseHelper()
        {
            // Fest integrierter Verbindungsstring und DataSource-Erstellung
            var connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=breakingbank";
            _dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _dataSource != null)
                {
                    _dataSource.Dispose();
                }

                _disposed = true;
            }
        }

        ~DatabaseHelper()
        {
            Dispose(false);
        }
    }
}