using Raylib_cs;

namespace Fotbal.App.Components;

internal class Window : BaseComponent
{
    private List<BaseComponent> _components;

    public Window()
    {
        _components = new List<BaseComponent>();
    }

    public void AddComponent(BaseComponent component)
    {
        _components.Add(component);
    }

    public override void Render()
    {
        foreach (var component in _components)
        {
            component.Render();
        }

        Raylib.EndDrawing();
    }
}
