using Godot;

[GlobalClass]
public partial class Hint : CanvasLayer
{
    [Export]
    public Label text_label;

    [Export]
    public Label title_label;

    [Export]
    public Control container;

    public void SetText(string text)
    {
        text_label.Text = text;
    }

    public void SetTitle(string title)
    {
        title_label.Visible = !string.IsNullOrEmpty(title);
        title_label.Text = string.IsNullOrEmpty(title) ? string.Empty : $"{title}:";
    }

    public void SetPosition(Vector2 pos)
    {
        container.GlobalPosition = pos;
    }

    public Vector2 GetPosition()
    {
        return container.GlobalPosition;
    }

    public Vector2 GetSize()
    {
        return container.Size;
    }
}
