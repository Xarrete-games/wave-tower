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
        this._enabled = true;
    }

    public void set_relics(Godot.Collections.Array<Variant> relics)
    {
        foreach (Node child in this.cards_container.GetChildren())
        {
            child.QueueFree();
        }

        foreach (Variant relicData in relics)
        {
            ChooseRelicCard card = ChooseRelicCardScene.Instantiate<ChooseRelicCard>();
            this.cards_container.AddChild(card);
            card.set_relic(relicData);
            card.card_pressed += this.OnCardPressed;
        }

        this.reroll_priece.price = this._rerollPrice;
    }

    private void OnCardPressed(Variant relicData)
    {
        if (!this._enabled)
        {
            return;
        }

        this.item_selected?.Invoke(relicData);
    }

    private void _on_reroll_button_xarreta_pressed()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int gold = runContext.economy.gold;
        if (this._rerollPrice <= gold)
        {
            this.reroll_pressed?.Invoke();
        }
    }

    private void _on_exit_button_xarreta_pressed()
    {
        QueueFree();
    }
}