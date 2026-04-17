using Godot;

[GlobalClass]
public partial class GoldCounter : HBoxContainer
{
    private const float CountingSpeed = 0.1f;

    private Label _amountGoldLabel;
    private Economy _economy;
    private float _currentDisplayedGold;
    private int _targetGold;

    public override void _Ready()
    {
        _amountGoldLabel = GetNode<Label>("AmountGoldLabel");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _economy = runContext.economy;

        _targetGold = _economy.gold;
        _currentDisplayedGold = _economy.gold;

        _economy.gold_change += OnGoldChange;
        UpdateLabel();
    }

    public override void _ExitTree()
    {
        if (_economy != null)
        {
            _economy.gold_change -= OnGoldChange;
        }
    }

    public override void _Process(double delta)
    {
        _currentDisplayedGold = Mathf.Lerp(_currentDisplayedGold, _targetGold, CountingSpeed);
        UpdateLabel();

        if (Mathf.Abs(_currentDisplayedGold - _targetGold) < 0.01f)
        {
            _currentDisplayedGold = _targetGold;
        }
    }

    private void OnGoldChange(int amount)
    {
        _targetGold = amount;
    }

    private void UpdateLabel()
    {
        _amountGoldLabel.Text = Mathf.RoundToInt(_currentDisplayedGold).ToString();
    }
}
