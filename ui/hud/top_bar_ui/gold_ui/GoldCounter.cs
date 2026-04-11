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
        this._amountGoldLabel = GetNode<Label>("AmountGoldLabel");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._economy = runContext.economy;

        this._targetGold = this._economy.gold;
        this._currentDisplayedGold = this._economy.gold;

        this._economy.gold_change += this._on_gold_change;
        this._update_label();
    }

    public override void _ExitTree()
    {
        if (this._economy != null)
        {
            this._economy.gold_change -= this._on_gold_change;
        }
    }

    public override void _Process(double delta)
    {
        this._currentDisplayedGold = Mathf.Lerp(this._currentDisplayedGold, this._targetGold, CountingSpeed);
        this._update_label();

        if (Mathf.Abs(this._currentDisplayedGold - this._targetGold) < 0.01f)
        {
            this._currentDisplayedGold = this._targetGold;
        }
    }

    private void _on_gold_change(int amount)
    {
        this._targetGold = amount;
    }

    private void _update_label()
    {
        this._amountGoldLabel.Text = Mathf.RoundToInt(this._currentDisplayedGold).ToString();
    }
}
