using Godot;

[GlobalClass]
public partial class Explanation : PanelContainer
{
    private static readonly PackedScene MainMenuScene = GD.Load<PackedScene>("uid://4i6kl0xurgeg");

    private void OnXarretaMenuButtonXarretaPressed()
    {
        GetTree().ChangeSceneToPacked(MainMenuScene);
    }
}
