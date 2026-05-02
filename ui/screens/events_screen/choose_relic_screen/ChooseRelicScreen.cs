using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class ChooseRelicScreen : Control
{
    public event Action<RelicData> ItemSelected;
    public event Action RerollPressed;

    private static readonly PackedScene ChooseRelicCardScene = GD.Load<PackedScene>("uid://dgcv5fdqvfext");

    [Export]
    public Control CardsContainer;

    [Export]
    public GoldPrice RerollPrice;

    private bool _enabled;
    private int _rerollPrice = 20;

    public override void _Ready()
    {
        AsyncTaskHelper.FireAndForget(ReadyAsync(), "ChooseRelicScreen.ReadyAsync");
    }

    private async Task ReadyAsync()
    {
        await ToSignal(GetTree().CreateTimer(0.3f, false), SceneTreeTimer.SignalName.Timeout);
        _enabled = true;
    }

    public void SetRelics(List<RelicData> relics)
    {
        foreach (Node child in CardsContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (RelicData relicData in relics)
        {
            ChooseRelicCard card = ChooseRelicCardScene.Instantiate<ChooseRelicCard>();
            CardsContainer.AddChild(card);
            card.SetRelic(relicData);
            card.CardPressed += OnCardPressed;
        }

        RerollPrice.Price = _rerollPrice;
    }

    private void OnCardPressed(RelicData relicData)
    {
        if (!_enabled)
        {
            return;
        }

        ItemSelected?.Invoke(relicData);
    }

    private void OnRerollButtonXarretaPressed()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int gold = runContext.Economy.Gold;
        if (_rerollPrice <= gold)
        {
            RerollPressed?.Invoke();
        }
    }

    private void OnExitButtonXarretaPressed()
    {
        QueueFree();
    }
}