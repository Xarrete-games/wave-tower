using Godot;

[GlobalClass]
public partial class HealthPrice : HBoxContainer
{
    private static readonly LabelSettings LabelSettings24Invalid = GD.Load<LabelSettings>("uid://c0seek6x1jue3");
    private static readonly LabelSettings LabelSettings24 = GD.Load<LabelSettings>("uid://bqa8xh2lpphdf");

    private bool _hasEnoughHealth;
    private Label _priceLabel;
    private RunContext _runContext;
    private int _price;

    public int price
    {
        get => _price;
        set
        {
            _price = value;
            if (_priceLabel != null)
            {
                _priceLabel.Text = value.ToString();
                CheckLabelColor(_runContext?.Status?.Health ?? 0);
            }
        }
    }

    public override void _Ready()
    {
        _priceLabel = GetNode<Label>("PriceLabel");
        _runContext = GetNode<RunContext>("/root/RunContext");
        _runContext.Status.HealthChanged += CheckLabelColor;
        CheckLabelColor(_runContext.Status.Health);
    }

    public override void _ExitTree()
    {
        if (_runContext?.Status != null)
        {
            _runContext.Status.HealthChanged -= CheckLabelColor;
        }
    }

    private void CheckLabelColor(int health)
    {
        _hasEnoughHealth = health > _price;
        _priceLabel.LabelSettings = _hasEnoughHealth ? LabelSettings24 : LabelSettings24Invalid;
    }
}
