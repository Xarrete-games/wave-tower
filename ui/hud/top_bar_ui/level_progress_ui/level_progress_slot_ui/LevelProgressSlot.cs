using Godot;

[GlobalClass]
public partial class LevelProgressSlot : PanelContainer
{
    private static readonly StyleBox LevelProgressSlotFill = GD.Load<StyleBox>("uid://ca11cx2n7wajo");
    private static readonly StyleBox ProgressSlotEmpty = GD.Load<StyleBox>("uid://d37mrhcqm5byv");

    [Export]
    public TextureRect texture_rec;

    public override void _Ready()
    {
        AddThemeStyleboxOverride("panel", ProgressSlotEmpty);
    }

    public void set_icon(Texture2D newTexture)
    {
        texture_rec.Texture = newTexture;
    }

    public void fill()
    {
        AddThemeStyleboxOverride("panel", LevelProgressSlotFill);
    }
}
