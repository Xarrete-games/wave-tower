using Godot;

[GlobalClass]
public partial class Hint : CanvasLayer
{
    [Export]
    public Label TextLabel;

    [Export]
    public Label TitleLabel;

    [Export]
    public Control container;

    public void SetText(string text)
    {
        TextLabel.Text = text;
    }

    public void SetTitle(string title)
    {
        TitleLabel.Visible = !string.IsNullOrEmpty(title);
        TitleLabel.Text = string.IsNullOrEmpty(title) ? string.Empty : $"{title}:";
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
