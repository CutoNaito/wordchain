using System.Text;
using Raylib_cs;
using Fotbal.Base;

namespace Fotbal.App.Components;

internal class TextInput : BaseComponent
{
    private int _maxChars;
    private Position _pos;
    private bool _isActive;
    private string _value;

    public Position Pos
    {
        get { return _pos; }
    }

    public TextInput(Position pos, int maxChars)
    {
        _pos = pos;
        _maxChars = maxChars;
        _isActive = false;
        _value = "";
    }

    public void ChangeValue(StringBuilder value)
    {
        _value = value.ToString();
        var mouse = Raylib.GetMousePosition();

        if (mouse.X >= _pos.X && mouse.X <= _pos.X + _pos.Width && mouse.Y >= _pos.Y && mouse.Y <= _pos.Y + _pos.Height)
        {
            _isActive = true;
        }
        else
        {
            _isActive = false;
        }

        if (_isActive)
        {
            int key = Raylib.GetCharPressed();

            while (key > 0)
            {
                if (key >= 32 && key <= 125 && value.Length < _maxChars)
                {
                    value.Append((char)key);
                }

                key = Raylib.GetCharPressed();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && value.Length > 0)
            {
                value.Remove(value.Length - 1, 1);
            }
        }
    }

    public override void Render()
    {
        Color border = _isActive ? Color.Red : Color.Gray;
        Raylib.DrawRectangleLines((int)_pos.X, (int)_pos.Y, (int)_pos.Width, (int)_pos.Height, border);
        Raylib.DrawText(_value.ToString(), (int)_pos.X + 10, (int)_pos.Y + 5, 20, Color.Black);
    }
}
