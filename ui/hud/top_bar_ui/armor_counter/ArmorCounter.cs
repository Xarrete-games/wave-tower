using Godot;

[GlobalClass]
public partial class ArmorCounter : CenterContainer
{
    private Label _counterLabel;
    private Status _status;
    private int _armor;

    public int armor
    {
        get => this._armor;
        set
        {
            this._armor = value;
            if (this._counterLabel != null)
            {
                this._counterLabel.Text = value.ToString();
            }
        }
    }

    public override void _Ready()
    {
        this._counterLabel = GetNode<Label>("CounterLabel");
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._status = runContext.status;
        this._status.armor_change += this._on_armor_change;
        this.armor = this._status.armor;
    }

    public override void _ExitTree()
    {
        if (this._status != null)
        {
            this._status.armor_change -= this._on_armor_change;
        }
    }

    private void _on_armor_change(int value)
    {
        this.armor = value;
    }
}
