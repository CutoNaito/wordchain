using Fotbal.Base;
using Fotbal.DB;
using Fotbal.App;
using Microsoft.Data.Sqlite;

var con = new Connection("Words");

con.Test();
con.CreateTable();
con.InsertWords();

MainLoop ml = new MainLoop(1200, 800, new SqliteConnection(con.ConnectionString));
await ml.StartGame();
