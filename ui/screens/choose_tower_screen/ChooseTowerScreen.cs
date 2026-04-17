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
        ok_button.Pressed += OnOkPressed;
    }

    public void PopulateScreen(List<TowerDataWithInstance> configurations)
    {
        foreach (TowerDataWithInstance towerConfiguration in configurations)
        {
            ChooseTowerScreenItem item = TowerItemScene.Instantiate<ChooseTowerScreenItem>();
            cards_container.AddChild(item);
            item.SetTowerData(towerConfiguration);
            item.selected += OnItemSelected;
        }
    }

    private void OnItemSelected(ChooseTowerScreenItem item)
    {
        _selectedItem = item;
        GetNode<AudioManager>("/root/AudioManager").play_tower_obtain();
        OnOkPressed();
    }

    private void OnOkPressed()
    {
        if (_selectedItem == null)
        {
            return;
        }

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager.OnTowerCardAdded(_selectedItem.GetTowerData());

        done?.Invoke();
        QueueFree();
    }
}