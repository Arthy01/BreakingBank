// Contains only SaveGame related logic of the DatabaseHelper

namespace BreakingBank.Helpers
{
    public partial class DatabaseHelper
    {
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
    }
}
