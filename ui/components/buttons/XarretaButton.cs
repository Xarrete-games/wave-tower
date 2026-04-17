using Godot;

[GlobalClass]
public partial class XarretaButton : Button
{
    [Export]
    public Theme theme_override;

    [Export]
    public Theme disabled_theme_override;

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
        if (disabled_theme_override != null)
        {
            Theme = disabled_theme_override;
        }
    }

    public void enable()
    {
        Disabled = false;
        if (theme_override != null)
        {
            Theme = theme_override;
        }
    }
}
