using System.Text;
using Fotbal.DB;
using Fotbal.DB.Models;
using Microsoft.Data.Sqlite;

namespace Fotbal.Base;

class Player : BaseObject
{
    public override StringBuilder Name { get; set; }

    public Player(string name)
    {
        Name = new StringBuilder(64);
        Name.Append(name);
    }

    private string Guess(string previous, string guessed)
    {
        if (previous[previous.Length - 1] == guessed[0])
        {
            return guessed;
        }

        return String.Empty;
    }

    private async Task<bool> Validate(string previous, string guessed, SqliteConnection conn)
    {
        await conn.OpenAsync();

        try
        {
            var record = await Words.ReadByWord(guessed, conn);

            if (record.Word != null)
            {
                if (previous != String.Empty)
                {
                    if (Guess(previous, record.Word) == String.Empty) {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async override Task<string> Play(string previous, SqliteConnection conn, string guessed = "")
    {
        if (await Validate(previous, guessed, conn))
        {
            return guessed;
        }
        else 
        {
            return "-1";
        }
    }
}
