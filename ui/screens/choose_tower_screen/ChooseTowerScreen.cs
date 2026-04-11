using Godot;

public partial class ChooseTowerScreen : Control
{
    private static readonly PackedScene TowerItemScene = GD.Load<PackedScene>("uid://cw14sfvcipnum");

    [Signal]
    public delegate void doneEventHandler();

    [Export]
    public Control cards_container;

    [Export]
    public Button ok_button;

    private ChooseTowerScreenItem _selectedItem;

    public override void _Ready()
    {
        this.ok_button.Pressed += this.OnOkPressed;
    }

    public void PopulateScreen(Godot.Collections.Array<Variant> configurations)
    {
        foreach (Variant towerConfiguration in configurations)
        {
            ChooseTowerScreenItem item = TowerItemScene.Instantiate<ChooseTowerScreenItem>();
            this.cards_container.AddChild(item);
            item.SetTowerData(towerConfiguration);
            item.selected += this.OnItemSelected;
        }
    }

    private void OnItemSelected(ChooseTowerScreenItem item)
    {
        this._selectedItem = item;
        GetNode<Node>("/root/AudioManager").Call("play_tower_obtain");
        this.OnOkPressed();
    }

    private void OnOkPressed()
    {
        if (this._selectedItem == null)
        {
            return;
        }

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager.Call("_on_tower_card_added", this._selectedItem.GetTowerData());

        EmitSignal(SignalName.done);
        QueueFree();
    }
}