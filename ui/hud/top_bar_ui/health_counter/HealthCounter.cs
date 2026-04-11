using Godot;

public partial class HealthCounter : HBoxContainer
{
    private int _health;
    private int _maxHealth;

    private Label _counterMaxLabel;
    private Label _counterLabel;
    private Control _armorCounter;
    private Status _status;

    private int health
    {
        get => this._health;
        set
        {
            this._health = value;
            this._counterLabel.Text = value.ToString();
        }
    }

    private int max_health
    {
        get => this._maxHealth;
        set
        {
            this._maxHealth = value;
            this._counterMaxLabel.Text = value.ToString();
        }
    }

    public override void _Ready()
    {
        this._counterMaxLabel = GetNode<Label>("CounterMaxLabel");
        this._counterLabel = GetNode<Label>("CounterLabel");
        this._armorCounter = GetNode<Control>("HBoxContainer/ArmorCounter");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._status = runContext.status;

        this._status.health_change += this._on_health_change;
        this._status.max_health_change += this._on_max_health_change;
        this._status.armor_change += this._on_armor_change;

        this.max_health = this._status.max_health;
        this.health = this._status.health;
        this._on_armor_change(this._status.armor);
    }

    public override void _ExitTree()
    {
        if (this._status != null)
        {
            this._status.health_change -= this._on_health_change;
            this._status.max_health_change -= this._on_max_health_change;
            this._status.armor_change -= this._on_armor_change;
        }
    }

    private void _on_health_change(int value)
    {
        this.health = value;
    }

    private void _on_max_health_change(int value)
    {
        this.max_health = value;
    }

    private void _on_armor_change(int amount)
    {
        this._armorCounter.Visible = amount > 0;
    }
}
