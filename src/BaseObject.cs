using System.Text;
using Microsoft.Data.Sqlite;

namespace Fotbal.Base;

abstract class BaseObject
{
    public virtual StringBuilder Name { get; set; }
    public abstract Task<string> Play(string previous, SqliteConnection conn, string guessed = "");
}
