using Godot;

[GlobalClass]
public partial class Explanation : PanelContainer
{
    private static readonly PackedScene MainMenuScene = GD.Load<PackedScene>("uid://4i6kl0xurgeg");

    private void _on_xarreta_menu_button_xarreta_pressed()
    {
        GetTree().ChangeSceneToPacked(MainMenuScene);
    }
}
