using Godot;

[GlobalClass]
public partial class LevelProgressSlot : PanelContainer
{
    private static readonly StyleBox LevelProgressSlotFill = GD.Load<StyleBox>("uid://ca11cx2n7wajo");
    private static readonly StyleBox ProgressSlotEmpty = GD.Load<StyleBox>("uid://d37mrhcqm5byv");

    [Export]
    public TextureRect TextureRec;

    public override void _Ready()
    {
        AddThemeStyleboxOverride("panel", ProgressSlotEmpty);
    }

    public void SetIcon(Texture2D newTexture)
    {
        TextureRec.Texture = newTexture;
    }

    public void Fill()
    {
        AddThemeStyleboxOverride("panel", LevelProgressSlotFill);
    }
}
