using Npgsql;

namespace BreakingBank.Helpers
{
    public class DatabaseHelper : IDisposable
    {
        private readonly NpgsqlDataSource _dataSource;
        private bool _disposed = false;

        public DatabaseHelper()
        {
            var connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=breakingbank";
            _dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public async Task GetAllUsers()
        {
            await using var command = _dataSource.CreateCommand("SELECT * FROM user_data");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetString(1));
                Console.WriteLine(reader.GetString(2));
            }
        }

        public async Task GetUserById(int userId)
        {
            await using var command = _dataSource.CreateCommand("SELECT * FROM user_data WHERE user_id = @userId");
            command.Parameters.AddWithValue("userId", userId);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetString(1));
                Console.WriteLine(reader.GetString(2));
            }
        }
        public async Task CreateUser(string username, string password)
        {
            await using var command = _dataSource.CreateCommand("INSERT INTO user_data (username, password) VALUES (@username, @password)");
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("password", password);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateUser(int userId, string username, string password)
        {
            await using var command = _dataSource.CreateCommand("UPDATE user_data SET username = @username, password = @password WHERE user_id = @userId");
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("password", password);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteUser(int userId)
        {
            await using var command = _dataSource.CreateCommand("DELETE FROM user_data WHERE user_id = @userId");
            command.Parameters.AddWithValue("userId", userId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task GetAllSavegames()
        {
            await using var command = _dataSource.CreateCommand("SELECT * FROM savegames");
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetInt32(1));
                Console.WriteLine(reader.GetString(2));
            }
        }

        public async Task GetSavegamesByUserId(int userId)
        {
            await using var command = _dataSource.CreateCommand("SELECT * FROM savegames WHERE user_id = @userId");
            command.Parameters.AddWithValue("userId", userId);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetInt32(1));
                Console.WriteLine(reader.GetString(2));
            }
        }

        public async Task CreateSavegame(int userId, string savegame)
        {
            await using var command = _dataSource.CreateCommand("INSERT INTO savegames (user_id, data) VALUES (@userId, @savegame)");
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("savegame", savegame);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateSavegame(int savegameId, string savegame)
        {
            await using var command = _dataSource.CreateCommand("UPDATE savegames SET data = @savegame WHERE savegame_id = @savegameId");
            command.Parameters.AddWithValue("savegameId", savegameId);
            command.Parameters.AddWithValue("savegame", savegame);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteSavegame(int savegameId)
        {
            await using var command = _dataSource.CreateCommand("DELETE FROM savegames WHERE savegame_id = @savegameId");
            command.Parameters.AddWithValue("savegameId", savegameId);
            await command.ExecuteNonQueryAsync();
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
