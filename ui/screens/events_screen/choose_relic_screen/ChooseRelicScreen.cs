using Godot;
using System.Threading.Tasks;

public partial class ChooseRelicScreen : Control
{
    [Signal]
    public delegate void item_selectedEventHandler(Variant item);

    [Signal]
    public delegate void reroll_pressedEventHandler();

    private static readonly PackedScene ChooseRelicCardScene = GD.Load<PackedScene>("uid://dgcv5fdqvfext");

    [Export]
    public Control cards_container;

    [Export]
    public Node reroll_priece;

    private bool _enabled;
    private int _rerollPrice = 20;

    public override async void _Ready()
    {
        await ToSignal(GetTree().CreateTimer(0.3f, false), "timeout");
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
            Node card = ChooseRelicCardScene.Instantiate();
            this.cards_container.AddChild(card);
            card.Call("set_relic", relicData);
            card.Connect("card_pressed", Callable.From<Variant>(this.OnCardPressed));
        }

        this.reroll_priece.Set("price", this._rerollPrice);
    }

    private void OnCardPressed(Variant relicData)
    {
        if (!this._enabled)
        {
            return;
        }

        EmitSignal(SignalName.item_selected, relicData);
    }

    private void _on_reroll_button_xarreta_pressed()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int gold = (int)runContext.economy.Get("gold");
        if (this._rerollPrice <= gold)
        {
            EmitSignal(SignalName.reroll_pressed);
        }
    }

    private void _on_exit_button_xarreta_pressed()
    {
        QueueFree();
    }
}