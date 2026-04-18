using Godot;

[GlobalClass]
public partial class XarretaButton : Button
{
    [Export]
    public Theme ThemeOverride;

    [Export]
    public Theme DisabledThemeOverride;

    private void OnMouseEntered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
    }

    private void OnMouseExited()
    {
    }

    private void OnPressed()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_click();
    }

    public void disable()
    {
        Disabled = true;
        if (DisabledThemeOverride != null)
        {
            Theme = DisabledThemeOverride;
        }
    }

    public void enable()
    {
        Disabled = false;
        if (ThemeOverride != null)
        {
            Theme = ThemeOverride;
        }
    }
}
