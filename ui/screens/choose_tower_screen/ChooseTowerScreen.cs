using Godot;
using System;
using System.Collections.Generic;

public partial class ChooseTowerScreen : Control
{
    private static readonly PackedScene TowerItemScene = GD.Load<PackedScene>("uid://cw14sfvcipnum");
    public event Action done;

    [Export]
    public Control cards_container;

    [Export]
    public Button ok_button;

    private ChooseTowerScreenItem _selectedItem;

    public override void _Ready()
    {
        this.ok_button.Pressed += this.OnOkPressed;
    }

    public void PopulateScreen(List<TowerDataWithInstance> configurations)
    {
        foreach (TowerDataWithInstance towerConfiguration in configurations)
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
        GetNode<AudioManager>("/root/AudioManager").play_tower_obtain();
        this.OnOkPressed();
    }

    private void OnOkPressed()
    {
        if (this._selectedItem == null)
        {
            return;
        }

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager._on_tower_card_added(this._selectedItem.GetTowerData());

        this.done?.Invoke();
        QueueFree();
    }
}