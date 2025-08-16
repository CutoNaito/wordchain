using Raylib_cs;

namespace Fotbal.App.Components;

internal class TextArea : BaseComponent
{
    private Position _pos;
    private string[] _lines;
    private int _maxLines;

    public Position Pos
    {
        get { return _pos; }
    }

    public TextArea(Position pos, string[] lines, int maxLines)
    {
        _pos = pos;
        _lines = lines;
        _maxLines = maxLines;
    }

    public override void Render()
    {
        Raylib.DrawRectangleLines((int)_pos.X, (int)_pos.Y, (int)_pos.Width, (int)_pos.Height, Color.Gray);

        float lineHeight = _pos.Height / _maxLines;
        int fontSize = (int)(lineHeight * 0.8f);

        for (int i = 0; i < _lines.Length; i++)
        {
            string text = _lines[i];
            int textWidth = Raylib.MeasureText(text, fontSize);
            float textX = _pos.X + (_pos.Width - textWidth) / 2;
            float textY = _pos.Y + i * lineHeight + (lineHeight - fontSize) / 2;

            Raylib.DrawText(text, (int)textX, (int)textY, fontSize, Color.Black);
        }
    }
}
