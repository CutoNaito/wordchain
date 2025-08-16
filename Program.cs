using Fotbal.Base;
using Fotbal.DB;
using Fotbal.App;
using Microsoft.Data.Sqlite;

var con = new Connection("Words");

con.Test();
con.CreateTable();
con.InsertWords();
// 
// Player p1 = new Player("jarda");
// Player p2 = new Player("honza");
// 
// TwoPlayerGame tp = new TwoPlayerGame(p1, p2);
// 
// tp.Start(new SqliteConnection(con.ConnectionString));

MainLoop ml = new MainLoop(1200, 800, new SqliteConnection(con.ConnectionString));
await ml.StartGame();
