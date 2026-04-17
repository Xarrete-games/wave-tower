using Godot;
using System;
using System.Threading.Tasks;

public partial class ChooseRelicScreen : Control
{
    public event Action<Variant> item_selected;
    public event Action reroll_pressed;

    private static readonly PackedScene ChooseRelicCardScene = GD.Load<PackedScene>("uid://dgcv5fdqvfext");

    [Export]
    public Control cards_container;

    [Export]
    public GoldPrice reroll_priece;

    private bool _enabled;
    private int _rerollPrice = 20;

    public override async void _Ready()
    {
        await ToSignal(GetTree().CreateTimer(0.3f, false), SceneTreeTimer.SignalName.Timeout);
        _enabled = true;
    }

    public void set_relics(Godot.Collections.Array<Variant> relics)
    {
        foreach (Node child in cards_container.GetChildren())
        {
            child.QueueFree();
        }

        foreach (Variant relicData in relics)
        {
            ChooseRelicCard card = ChooseRelicCardScene.Instantiate<ChooseRelicCard>();
            cards_container.AddChild(card);
            card.set_relic(relicData);
            card.card_pressed += OnCardPressed;
        }

        reroll_priece.price = _rerollPrice;
    }

    private void OnCardPressed(Variant relicData)
    {
        if (!_enabled)
        {
            return;
        }

        item_selected?.Invoke(relicData);
    }

    private void OnRerollButtonXarretaPressed()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int gold = runContext.economy.gold;
        if (_rerollPrice <= gold)
        {
            reroll_pressed?.Invoke();
        }
    }

    private void OnExitButtonXarretaPressed()
    {
        QueueFree();
    }
}