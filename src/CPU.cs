using System.Text;
using Fotbal.DB;
using Fotbal.DB.Models;
using Microsoft.Data.Sqlite;

namespace Fotbal.Base;

class CPU : BaseObject
{
    private int _diff;

    public override StringBuilder Name { get; set; } = new StringBuilder("CPU");

    public CPU(int diff)
    {
        _diff = diff;
    }

    private async Task<string> GuessCorrect(string previous, SqliteConnection conn)
    {
        char letter = previous[previous.Length - 1];

        try
        {
            var record = await Words.GetRandomWord(letter, conn);

            if (record != null)
            {
                return record.Word;
            }
            else
            {
                return "-1";
            }
        }
        catch (Exception e)
        {
            return "-1";
        }
    }

    private async Task<string> GuessRandom(SqliteConnection conn)
    {
        char letter = (char)(new Random()).Next(97, 122);

        try
        {
            var record = await Words.GetRandomWord(letter, conn);

            if (record != null)
            {
                return record.Word;
            }
            else
            {
                return "-1";
            }
        }
        catch (Exception e)
        {
            return "-1";
        }
    }

    public async override Task<string> Play(string previous, SqliteConnection conn, string guessed = "")
    {
        switch (_diff)
        {
            case 0:
                if ((new Random()).NextDouble() <= 0.5)
                {
                    return await GuessRandom(conn);
                }

                return await GuessCorrect(previous, conn);

            case 1:
                if ((new Random()).NextDouble() <= 0.3)
                {
                    return await GuessRandom(conn);
                }

                return await GuessCorrect(previous, conn);

            case 2:
                if ((new Random()).NextDouble() <= 0.1)
                {
                    return await GuessRandom(conn);
                }

                return await GuessCorrect(previous, conn);

            default:
                return String.Empty;
        }
    }
}
