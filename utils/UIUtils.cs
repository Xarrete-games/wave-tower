using Godot;

public static class UIUtilsStatic
{
    private static readonly Color[] DefaultColors =
    {
        new Color(0.12f, 0.16f, 0.14f, 1.0f),
        new Color(0.35f, 0.45f, 0.40f, 1.0f),
        new Color(0.80f, 0.74f, 0.42f, 1.0f),
    };

    private static bool _initialized;
    private static Color _primaryColor = DefaultColors[0];
    private static Color _secondaryColor = DefaultColors[1];
    private static Color _accentColor = DefaultColors[2];

    public static Color PrimaryColor
    {
        get
        {
            EnsureInitialized();
            return _primaryColor;
        }
    }

    public static Color SecondaryColor
    {
        get
        {
            EnsureInitialized();
            return _secondaryColor;
        }
    }

    public static Color AccentColor
    {
        get
        {
            EnsureInitialized();
            return _accentColor;
        }
    }

    public static bool IsLeftClickEvent(InputEvent inputEvent)
    {
        return InputClickUtils.IsLeftClickReleased(inputEvent);
    }

    public static bool IsRightClickEvent(InputEvent inputEvent)
    {
        return InputClickUtils.IsRightClickReleased(inputEvent);
    }

    public static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        Godot.ColorPalette palette = ResourceLoader.Load<Godot.ColorPalette>("uid://dbyjsuchchdht");
        if (palette == null || palette.Colors == null || palette.Colors.Length < 3)
        {
            return;
        }

        _primaryColor = palette.Colors[0];
        _secondaryColor = palette.Colors[1];
        _accentColor = palette.Colors[2];
    }
}

public partial class UIUtils : Node
{
    public Color primary_color => UIUtilsStatic.PrimaryColor;
    public Color secondary_color => UIUtilsStatic.SecondaryColor;
    public Color accent_color => UIUtilsStatic.AccentColor;

    public override void _Ready()
    {
        UIUtilsStatic.EnsureInitialized();
    }

    public bool is_left_click_event(InputEvent input_event)
    {
        return UIUtilsStatic.IsLeftClickEvent(input_event);
    }

    public bool is_right_click_event(InputEvent input_event)
    {
        return UIUtilsStatic.IsRightClickEvent(input_event);
    }
}