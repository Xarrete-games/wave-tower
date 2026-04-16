using Godot;

[GlobalClass]
public partial class XarretaButton : Button
{
    [Export]
    public Theme theme_override;

    [Export]
    public Theme disabled_theme_override;

    private void _on_mouse_entered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
    }

    private void _on_mouse_exited()
    {
    }

    private void _on_pressed()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_click();
    }

    public void disable()
    {
        this.Disabled = true;
        if (this.disabled_theme_override != null)
        {
            this.Theme = this.disabled_theme_override;
        }
    }

    public void enable()
    {
        this.Disabled = false;
        if (this.theme_override != null)
        {
            this.Theme = this.theme_override;
        }
    }
}
