using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class ResolutionMenu : VBoxContainer
{
    [Export]
    public OptionButton ResolutionOptionButton;

    [Export]
    public OptionButton ModeOptionButton;

    private readonly Dictionary<string, Vector2I> _resolutions = new()
    {
        { "3840x2160", new Vector2I(3840, 2160) },
        { "2560x1440", new Vector2I(2560, 1440) },
        { "1920x1080", new Vector2I(1920, 1080) },
        { "1600x900", new Vector2I(1600, 900) },
        { "1366x768", new Vector2I(1366, 768) },
        { "1280x720", new Vector2I(1280, 720) },
        { "800x600", new Vector2I(800, 600) },
    };

    private readonly List<string> _visibleResolutions = new();

    public override void _Ready()
    {
        PopulateResolutions();
        UpdateButtonValues();
    }

    public void UpdateButtonValues()
    {
        Vector2I windowSize = GetWindow().Size;
        string current = $"{windowSize.X}x{windowSize.Y}";

        int index = _visibleResolutions.IndexOf(current);
        if (index != -1)
        {
            ResolutionOptionButton.Selected = index;
        }
    }

    private void PopulateResolutions()
    {
        ResolutionOptionButton.Clear();
        _visibleResolutions.Clear();

        Vector2I screenSize = DisplayServer.ScreenGetSize();

        foreach (var entry in _resolutions)
        {
            string resName = entry.Key;
            Vector2I res = entry.Value;
            if (res.X <= screenSize.X && res.Y <= screenSize.Y)
            {
                _visibleResolutions.Add(resName);
                ResolutionOptionButton.AddItem(resName);
            }
        }
    }

    private void OnOptionButtonItemSelected(int index)
    {
        if (index < 0 || index >= _visibleResolutions.Count)
        {
            return;
        }

        string key = _visibleResolutions[index];
        GetWindow().Size = _resolutions[key];
        CenterWindow();
    }

    private void CenterWindow()
    {
        Vector2I screenCenter = DisplayServer.ScreenGetPosition() + (DisplayServer.ScreenGetSize() / 2);
        Vector2I windowSize = GetWindow().GetSizeWithDecorations();
        GetWindow().Position = screenCenter - (windowSize / 2);
    }
}
