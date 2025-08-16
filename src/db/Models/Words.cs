using Microsoft.Data.Sqlite;

namespace Fotbal.DB.Models;

internal class Words
{
    public int Id { get; set; }
    public string Word { get; set; }

    public static async Task<Words> ReadByWord(string word, SqliteConnection conn)
    {
        string sql = "SELECT * FROM Words WHERE LOWER(Word) = LOWER(@word);";
        await using (var cmd = new SqliteCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@word", word);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Words
                    {
                        Id = reader.GetInt32(0),
                        Word = reader.GetString(1)
                    };
                }
            }
        }

        return null;
    }

    public static async Task<Words> GetRandomWord(char letter, SqliteConnection conn)
    {
        string sql = "SELECT * FROM Words WHERE LOWER(Word) LIKE @letter || '%' ORDER BY RANDOM() LIMIT 1;";
        await using (var cmd = new SqliteCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@letter", letter);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Words
                    {
                        Id = reader.GetInt32(0),
                        Word = reader.GetString(1)
                    };
                }
            }
        }

        return null;
    }

}
