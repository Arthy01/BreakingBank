// Contains only SaveGame related logic of the DatabaseHelper

using BreakingBank.JsonConverters;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;

namespace BreakingBank.Helpers
{
    public partial class DatabaseHelper
    {
        public async Task<bool> CreateSaveGame(SaveGame saveGame)
        {
            await using NpgsqlCommand command = _dataSource.CreateCommand("INSERT INTO savegames (user_id, data, savegame_id) VALUES (@userId, @savegame, @savegame_id)");
            command.Parameters.AddWithValue("userId", saveGame.MetaData.OwnerUserID);
            command.Parameters.AddWithValue("savegame", NpgsqlDbType.Jsonb, SerializeSavegame(saveGame));
            command.Parameters.AddWithValue("savegame_id", Guid.Parse(saveGame.MetaData.ID));
            
            int affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        public async Task<bool> UpdateSaveGame(SaveGame saveGame)
        {
            await using NpgsqlCommand command = _dataSource.CreateCommand("UPDATE savegames SET data = @savegame WHERE savegame_id = @savegame_id");
            command.Parameters.AddWithValue("savegame", NpgsqlDbType.Jsonb, SerializeSavegame(saveGame));
            command.Parameters.AddWithValue("savegame_id", Guid.Parse(saveGame.MetaData.ID));

            int affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        public async Task<bool> SaveGameExists(string saveGameID)
        {
            await using NpgsqlCommand command = _dataSource.CreateCommand("SELECT 1 FROM savegames WHERE savegame_id = @savegame_id LIMIT 1");
            command.Parameters.AddWithValue("savegame_id", Guid.Parse(saveGameID));

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync(); // true if at least 1 row is found
        }

        public async Task<bool> DeleteSaveGame(SaveGame saveGame)
        {
            return await DeleteSaveGame(saveGame.MetaData.ID);
        }

        public async Task<bool> DeleteSaveGame(string saveGameID)
        {
            await using NpgsqlCommand command = _dataSource.CreateCommand("DELETE FROM savegames WHERE savegame_id = @savegame_id");
            command.Parameters.AddWithValue("savegame_id", Guid.Parse(saveGameID));

            int affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        public async Task<SaveGame?> GetSaveGame(string saveGameID)
        {
            await using NpgsqlCommand command = _dataSource.CreateCommand("SELECT * FROM savegames WHERE savegame_id = @savegame_id");
            command.Parameters.AddWithValue("savegame_id", NpgsqlDbType.Uuid, Guid.Parse(saveGameID));

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                string json = reader.GetString(1);

                if (SaveGame.Parse(json, out SaveGame saveGame))
                    return saveGame;

                return null;
            }

            return null;
        }

        public async Task<List<SaveGame>> GetAllSaveGamesByUser(User user)
        {
            List<SaveGame> saveGames = new();

            await using var command = _dataSource.CreateCommand(@"SELECT data FROM savegames WHERE user_id = @user_id");

            command.Parameters.AddWithValue("user_id", user.ID);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string json = reader.GetString(0);
                if (SaveGame.Parse(json, out SaveGame saveGame))
                    saveGames.Add(saveGame);
            }

            return saveGames;
        }

        private string SerializeSavegame(SaveGame saveGame)
        {
            JsonSerializerOptions options = new();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.Converters.Add(new DirtyFieldJsonConverterFactory());
            Console.WriteLine(JsonSerializer.Serialize(saveGame, options));
            return JsonSerializer.Serialize(saveGame, options);
        }


        /*
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
        */
    }
}
