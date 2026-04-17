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

    public void set_text(string text)
    {
        text_label.Text = text;
    }

    public void set_title(string title)
    {
        title_label.Visible = !string.IsNullOrEmpty(title);
        title_label.Text = string.IsNullOrEmpty(title) ? string.Empty : $"{title}:";
    }

    public void set_position(Vector2 pos)
    {
        container.GlobalPosition = pos;
    }

    public Vector2 get_position()
    {
        return container.GlobalPosition;
    }

    public Vector2 get_size()
    {
        return container.Size;
    }
}
