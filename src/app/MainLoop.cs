using System.Text;
using Microsoft.Data.Sqlite;
using Raylib_cs;
using Fotbal.App.Components;
using Fotbal.Base;

namespace Fotbal.App;

enum Screen
{
    MainMenu,
    TwoPlayerSettings,
    VsCPUSettings,
    Game,
    GameEnd,
}

enum Difficulty
{
    Easy,
    Normal,
    Hard,
}

class MainLoop
{
    private SqliteConnection _conn;
    private int _screenWidth;
    private int _screenHeight;
    private Screen _current;
    private BaseObject? _p1;
    private BaseObject? _p2;
    private BaseObject? _leader;
    private Difficulty _selectedCpuDiff;
    private StringBuilder _currentWord;
    private List<string> _guessedWords;
    private int _attempts;

    public MainLoop(int screenWidth, int screenHeight, SqliteConnection conn)
    {
        _conn = conn;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _currentWord = new StringBuilder(64);
        _current = Screen.MainMenu;
        _selectedCpuDiff = Difficulty.Easy;
        _guessedWords = new List<string>();
        _attempts = 3;

        Raylib.InitWindow(screenWidth, screenHeight, "Fotbal");
    }

    public async Task StartGame()
    {
        while (!Raylib.WindowShouldClose())
        {
            switch (_current)
            {
                case Screen.MainMenu:
                    {
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.RayWhite);

                        Window mainMenu = new Window();

                        Button playTwoGame = new Button("Player vs. Player", new Position(_screenWidth / 6, 200, _screenWidth / 4, _screenHeight / 4), () => 
                        {
                            _p1 = new Player(String.Empty);
                            _p2 = new Player(String.Empty);
                            _current = Screen.TwoPlayerSettings;
                        });

                        Button playCpu = new Button("Player vs. CPU", new Position(_screenWidth - (_screenWidth / 6) - (_screenWidth / 4), 200, _screenWidth / 4, _screenHeight / 4), () => 
                        {
                            _p1 = new Player(String.Empty);
                            _current = Screen.VsCPUSettings;
                        });

                        Button quit = new Button("Quit", new Position(_screenWidth / 4, 500, _screenWidth / 2, _screenHeight / 4), () =>
                        {
                            Raylib.CloseWindow();
                        });

                        mainMenu.AddComponent(playTwoGame);
                        mainMenu.AddComponent(playCpu);
                        mainMenu.AddComponent(quit);

                        mainMenu.Render();
                        
                        Raylib.EndDrawing();
                    }

                    break;

                case Screen.TwoPlayerSettings:
                    {
                        TextInput p1 = new TextInput(new Position(_screenWidth / 6, 200, _screenWidth / 4, _screenHeight / 4), 20);
                        TextInput p2 = new TextInput(new Position(_screenWidth - (_screenWidth / 6) - (_screenWidth / 4), 200, _screenWidth / 4, _screenHeight / 4), 20);

                        p1.ChangeValue(_p1.Name);
                        p2.ChangeValue(_p2.Name);

                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.RayWhite);

                        Window twoGameSettings = new Window();

                        Button start = new Button("Start", new Position(_screenWidth / 4, _screenHeight / 1.6f, _screenWidth / 2, _screenHeight / 8), () => 
                        {
                            // if (String.IsNullOrEmpty(_p1) || String.IsNullOrEmpty(_p2))
                            // {
                            // }
                            _current = Screen.Game;
                        });

                        Button back = new Button("Back", new Position(_screenWidth / 4, _screenHeight / 1.3f, _screenWidth / 2, _screenHeight / 8), () =>
                        {
                            _current = Screen.MainMenu;
                        });

                        Raylib.DrawText("Enter your names", (_screenWidth - Raylib.MeasureText("Enter your names", 20)) / 2, _screenHeight / 8, 20, Color.Black);

                        Raylib.DrawText("Player 1", (int)p1.Pos.X + ((int)p1.Pos.Width - Raylib.MeasureText("Player 1", 20)) / 2, _screenHeight / 5, 20, Color.Black);
                        Raylib.DrawText("Player 2", (int)p2.Pos.X + ((int)p2.Pos.Width - Raylib.MeasureText("Player 2", 20)) / 2, _screenHeight / 5, 20, Color.Black);

                        twoGameSettings.AddComponent(p1);
                        twoGameSettings.AddComponent(p2);
                        twoGameSettings.AddComponent(start);
                        twoGameSettings.AddComponent(back);

                        twoGameSettings.Render();

                        Raylib.EndDrawing();
                    }

                    break;

                case Screen.VsCPUSettings:
                    {
                        TextInput p1 = new TextInput(new Position(_screenWidth / 4, _screenHeight / 4, _screenWidth / 2, _screenHeight / 8), 20);

                        p1.ChangeValue(_p1.Name);

                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.RayWhite);

                        Window vsCPUSettings = new Window();

                        Raylib.DrawText("Enter your name", (_screenWidth - Raylib.MeasureText("Enter your name", 20)) / 2, _screenHeight / 6, 20, Color.Black);

                        Button diffEasy = new Button("Easy", new Position(_screenWidth / 5.454545f, _screenHeight / 2.2f, _screenWidth / 5, _screenHeight / 8), () =>
                        {
                            _selectedCpuDiff = Difficulty.Easy;
                        });

                        Button diffNormal = new Button("Normal", new Position(_screenWidth / 2.5f, _screenHeight / 2.2f, _screenWidth / 5, _screenHeight / 8), () => 
                        {
                            _selectedCpuDiff = Difficulty.Normal;
                        });

                        Button diffHard = new Button("Hard", new Position(_screenWidth / 1.62162162162f, _screenHeight / 2.2f, _screenWidth / 5, _screenHeight / 8), () =>
                        {
                            _selectedCpuDiff = Difficulty.Hard;
                        });

                        int difft1 = Raylib.MeasureText("Selected difficulty: ", 30);
                        int difft2 = Raylib.MeasureText("Normal", 30);
                        
                        Raylib.DrawText("Selected difficulty: ", (_screenWidth - difft1 - difft2) / 2, (int)(_screenHeight / 1.6f), 30, Color.Black);
                        Raylib.DrawText(_selectedCpuDiff.ToString(), (_screenWidth - difft2 + difft1) / 2, (int)(_screenHeight / 1.6f), 30, Color.Red);

                        Button start = new Button("Start", new Position(_screenWidth / 4, _screenHeight / 1.3f, _screenWidth / 2, _screenHeight / 8), () => 
                        {
                            _p2 = new CPU((int)_selectedCpuDiff);
                            _current = Screen.Game;
                        });

                        vsCPUSettings.AddComponent(p1);
                        vsCPUSettings.AddComponent(diffEasy);
                        vsCPUSettings.AddComponent(diffNormal);
                        vsCPUSettings.AddComponent(diffHard);
                        vsCPUSettings.AddComponent(start);

                        vsCPUSettings.Render();

                        Raylib.EndDrawing();
                    }

                    break;
                    
                case Screen.Game:
                    {
                        BaseObject currentPlayer;
                        string[] displayedWords;

                        if (_guessedWords.Count <= 4)
                        {
                            displayedWords = _guessedWords.ToArray().Reverse().ToArray();
                        }
                        else
                        {
                            displayedWords = _guessedWords.ToArray()[^4..].Reverse().ToArray();
                        }

                        if (_guessedWords.Count % 2 == 0)
                        {
                            currentPlayer = _p1;
                            _leader = _p2;
                        }
                        else
                        {
                            currentPlayer = _p2;
                            _leader = _p1;
                        }

                        TextInput wordInput = new TextInput(new Position(_screenWidth / 4, _screenHeight / 6, _screenWidth / 2, _screenHeight / 8), 30);

                        wordInput.ChangeValue(_currentWord);

                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.RayWhite);

                        Window twoGame = new Window();

                        Raylib.DrawText(currentPlayer.Name.ToString() + "'s turn", (_screenWidth - Raylib.MeasureText(currentPlayer.Name.ToString() + "'s turn", 20)) / 2, _screenHeight / 8, 20, Color.Black);

                        if (_p2.GetType().ToString() == "Fotbal.Base.CPU" && currentPlayer == _p2)
                        {
                            string guessedWord = await currentPlayer.Play(_guessedWords[_guessedWords.Count - 1], _conn);

                            if (guessedWord == "-1" || _guessedWords.Contains(guessedWord))
                            {
                                _attempts--;
                            }
                            else
                            {
                                _attempts = 3;
                                _guessedWords.Add(guessedWord);
                            }
                        }

                        Button submitWord = new Button("Submit", new Position(wordInput.Pos.X + wordInput.Pos.Width + 5, wordInput.Pos.Y, wordInput.Pos.Width / 3, wordInput.Pos.Height), async () => 
                        {
                            if (_guessedWords.Count == 0)
                            {
                                string guessedWord = await currentPlayer.Play(String.Empty, _conn, _currentWord.ToString().ToLower());

                                if (guessedWord == "-1")
                                {
                                    _attempts--;
                                }
                                else
                                {
                                    _attempts = 3;
                                    _guessedWords.Add(guessedWord);
                                }
                            }
                            else
                            {
                                string guessedWord = await currentPlayer.Play(_guessedWords[_guessedWords.Count - 1], _conn, _currentWord.ToString().ToLower());

                                if (guessedWord == "-1" || _guessedWords.Contains(guessedWord))
                                {
                                    _attempts--;
                                }
                                else
                                {
                                    _attempts = 3;
                                    _guessedWords.Add(guessedWord);
                                }
                            }
                        });

                        TextArea guessed = new TextArea(new Position(wordInput.Pos.X, wordInput.Pos.Y + wordInput.Pos.Height, wordInput.Pos.Width, wordInput.Pos.Height * 2), displayedWords, 4);

                        Raylib.DrawText("Attemps: " + _attempts, (_screenWidth - Raylib.MeasureText("Attempts: " + _attempts, 20)) / 2, (int)(guessed.Pos.Y + guessed.Pos.Height + 20), 20, Color.Black);

                        if (_attempts == 0)
                        {
                            _guessedWords = [];
                            _attempts = 3;
                            _current = Screen.GameEnd;
                        }

                        twoGame.AddComponent(wordInput);
                        twoGame.AddComponent(submitWord);
                        twoGame.AddComponent(guessed);

                        twoGame.Render();
                        
                        Raylib.EndDrawing();
                    }

                    break;

                case Screen.GameEnd:
                    {
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.RayWhite);

                        Raylib.DrawText("Game Over!", (_screenWidth - Raylib.MeasureText("Game Over!", 40)) / 2, _screenHeight / 8, 40, Color.Black);
                        Raylib.DrawText("Winner: " + _leader.Name, (_screenWidth - Raylib.MeasureText("Winner: " + _leader.Name, 30)) / 2, _screenHeight / 5, 30, Color.Black);

                        Window twoGameEnd = new Window();

                        Button menu = new Button("Back to Menu", new Position(_screenWidth / 4, _screenHeight / 1.5f, _screenWidth / 2, _screenHeight / 8), () => 
                        {
                            _current = Screen.MainMenu;
                        });

                        twoGameEnd.AddComponent(menu);

                        twoGameEnd.Render();

                        Raylib.EndDrawing();

                    }

                    break;
            }

        }

        Raylib.CloseWindow();
    }
}
