using Raylib_cs;

namespace Fotbal.App.Components;

internal class Button : BaseComponent
{
    private Rectangle _buttonBounds;
    private Position _pos;
    private string _text;
    private Action _callback;

    public Button(string text, Position pos, Action callback)
    {
        _pos = pos;
        _buttonBounds = new Rectangle(_pos.X, _pos.Y, _pos.Width, _pos.Height);
        _text = text;
        _callback = callback;
    }

    public override void Render()
    {
        bool isHovering = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _buttonBounds);
        bool isClicked = isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (isClicked)
        {
            _callback();
        }

        Color buttonColor = isHovering ? Color.LightGray : Color.Gray;
        Raylib.DrawRectangleRec(_buttonBounds, buttonColor);
        Raylib.DrawRectangleLinesEx(_buttonBounds, 2, Color.Black);

        // Draw text centered in button
        int fontSize = 20;
        int textWidth = Raylib.MeasureText(_text, fontSize);
        Raylib.DrawText(
                _text,
                (int)(_buttonBounds.X + (_buttonBounds.Width - textWidth) / 2),
                (int)(_buttonBounds.Y + (_buttonBounds.Height - fontSize) / 2),
                fontSize,
                Color.Black
                );
    }
}
