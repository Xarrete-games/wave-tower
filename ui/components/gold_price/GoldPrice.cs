using Godot;

[GlobalClass]
public partial class GoldPrice : HBoxContainer
{
    private static readonly LabelSettings LabelSettings24Invalid = GD.Load<LabelSettings>("uid://c0seek6x1jue3");
    private static readonly LabelSettings LabelSettings24 = GD.Load<LabelSettings>("uid://bqa8xh2lpphdf");

    private bool _hasEnoughGold;
    private Label _priceLabel;
    private RunContext _runContext;
    private int _price;

    public int price
    {
        get => this._price;
        set
        {
            this._price = value;
            if (this._priceLabel != null)
            {
                this._priceLabel.Text = value.ToString();
                this._check_label_color(this._runContext?.economy?.gold ?? 0);
            }
        }
    }

    public override void _Ready()
    {
        this._priceLabel = GetNode<Label>("PriceLabel");
        this._runContext = GetNode<RunContext>("/root/RunContext");

        this._priceLabel.Text = this._price.ToString();
        this._check_label_color(this._runContext.economy.gold);
        this._runContext.economy.gold_change += this._check_label_color;
    }

    public override void _ExitTree()
    {
        if (this._runContext?.economy != null)
        {
            this._runContext.economy.gold_change -= this._check_label_color;
        }
    }

    private void _check_label_color(int gold)
    {
        this._hasEnoughGold = gold >= this._price;
        this._priceLabel.LabelSettings = this._hasEnoughGold ? LabelSettings24 : LabelSettings24Invalid;
    }
}
