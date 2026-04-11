using Godot;

[GlobalClass]
public partial class ResolutionMenu : VBoxContainer
{
    [Export]
    public OptionButton resolution_option_button;

    [Export]
    public OptionButton mode_option_button;

    private readonly Godot.Collections.Dictionary<string, Vector2I> _resolutions = new()
    {
        { "3840x2160", new Vector2I(3840, 2160) },
        { "2560x1440", new Vector2I(2560, 1440) },
        { "1920x1080", new Vector2I(1920, 1080) },
        { "1600x900", new Vector2I(1600, 900) },
        { "1366x768", new Vector2I(1366, 768) },
        { "1280x720", new Vector2I(1280, 720) },
        { "800x600", new Vector2I(800, 600) },
    };

    private readonly Godot.Collections.Array<string> _visibleResolutions = new();

    public override void _Ready()
    {
        this._populate_resolutions();
        this.update_button_values();
    }

    public void update_button_values()
    {
        Vector2I windowSize = GetWindow().Size;
        string current = $"{windowSize.X}x{windowSize.Y}";

        int index = this._visibleResolutions.IndexOf(current);
        if (index != -1)
        {
            this.resolution_option_button.Selected = index;
        }
    }

    private void _populate_resolutions()
    {
        this.resolution_option_button.Clear();
        this._visibleResolutions.Clear();

        Vector2I screenSize = DisplayServer.ScreenGetSize();

        foreach (var entry in this._resolutions)
        {
            string resName = entry.Key;
            Vector2I res = entry.Value;
            if (res.X <= screenSize.X && res.Y <= screenSize.Y)
            {
                this._visibleResolutions.Add(resName);
                this.resolution_option_button.AddItem(resName);
            }
        }
    }

    private void _on_option_button_item_selected(int index)
    {
        if (index < 0 || index >= this._visibleResolutions.Count)
        {
            return;
        }

        string key = this._visibleResolutions[index];
        GetWindow().Size = this._resolutions[key];
        this.center_window();
    }

    private void center_window()
    {
        Vector2I screenCenter = DisplayServer.ScreenGetPosition() + (DisplayServer.ScreenGetSize() / 2);
        Vector2I windowSize = GetWindow().GetSizeWithDecorations();
        GetWindow().Position = screenCenter - (windowSize / 2);
    }
}
