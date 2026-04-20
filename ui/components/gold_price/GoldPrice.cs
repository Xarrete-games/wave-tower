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

    public int Price
    {
        get => _price;
        set
        {
            _price = value;
            if (_priceLabel != null)
            {
                _priceLabel.Text = value.ToString();
                CheckLabelColor(_runContext?.Economy?.Gold ?? 0);
            }
        }
    }

    public override void _Ready()
    {
        _priceLabel = GetNode<Label>("PriceLabel");
        _runContext = GetNode<RunContext>("/root/RunContext");

        _priceLabel.Text = _price.ToString();
        CheckLabelColor(_runContext.Economy.Gold);
        _runContext.Economy.GoldChanged += CheckLabelColor;
    }

    public override void _ExitTree()
    {
        if (_runContext?.Economy != null)
        {
            _runContext.Economy.GoldChanged -= CheckLabelColor;
        }
    }

    private void CheckLabelColor(int gold)
    {
        _hasEnoughGold = gold >= _price;
        _priceLabel.LabelSettings = _hasEnoughGold ? LabelSettings24 : LabelSettings24Invalid;
    }
}
