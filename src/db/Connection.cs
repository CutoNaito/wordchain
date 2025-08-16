using System.Configuration;
using Microsoft.Data.Sqlite;

namespace Fotbal.DB;

internal class Connection
{
    string _connectionString;

    public string ConnectionString
    {
        get { return _connectionString; }
    }

    public Connection(string db)
    {
        _connectionString = ConfigurationManager.ConnectionStrings[db].ConnectionString;
    }

    public void Test()
    {
        try
        {
            var con = new SqliteConnection(_connectionString);
            con.Open();
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void CreateTable()
    {
        string sql = "CREATE TABLE IF NOT EXISTS Words (" +
                     "ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                     "Word TEXT NOT NULL);";

        try
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public void InsertWords()
    {
        var words = File.ReadAllLines("words.txt");

        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();

            using (var cmd = new SqliteCommand("PRAGMA synchronous = OFF", conn)) cmd.ExecuteNonQuery();
            using (var cmd = new SqliteCommand("PRAGMA journal_mode = MEMORY", conn)) cmd.ExecuteNonQuery();
            
            string sql = "INSERT INTO Words (Word) VALUES (@word);";

            using (var trans = conn.BeginTransaction())
            using (var cmd = new SqliteCommand(sql, conn, trans))
            {
                var param = cmd.Parameters.Add("@word", SqliteType.Text);

                foreach (var word in words)
                {
                    param.Value = word.ToLower();
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }

            conn.Close();
        }
    }

    public static async Task<String> ReadByWord(string word, SqliteConnection conn)
    {
        string sql = "SELECT Word FROM Words WHERE Word = @word;";
        await using (var cmd = new SqliteCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@word", word);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return reader.GetString(0);
                }
            }
        }

        return null;
    }
}
