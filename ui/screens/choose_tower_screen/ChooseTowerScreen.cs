using Godot;
using System;
using System.Collections.Generic;

public partial class ChooseTowerScreen : Control
{
    private static readonly PackedScene TowerItemScene = GD.Load<PackedScene>("uid://cw14sfvcipnum");
    public event Action done;

    [Export]
    public Control CardsContainer;

    [Export]
    public Button OkButton;

    private ChooseTowerScreenItem _selectedItem;

    public override void _Ready()
    {
        OkButton.Pressed += OnOkPressed;
    }

    public void PopulateScreen(List<TowerDataWithInstance> configurations)
    {
        foreach (TowerDataWithInstance towerConfiguration in configurations)
        {
            ChooseTowerScreenItem item = TowerItemScene.Instantiate<ChooseTowerScreenItem>();
            CardsContainer.AddChild(item);
            item.SetTowerData(towerConfiguration);
            item.selected += OnItemSelected;
        }
    }

    private void OnItemSelected(ChooseTowerScreenItem item)
    {
        _selectedItem = item;
        GetNode<AudioManager>("/root/AudioManager").PlayTowerObtain();
        OnOkPressed();
    }

    private void OnOkPressed()
    {
        if (_selectedItem == null)
        {
            return;
        }

        RunContext runContext = RunContext.Instance;
        runContext.TowersManager.OnTowerCardAdded(_selectedItem.GetTowerData());

        done?.Invoke();
        QueueFree();
    }
}