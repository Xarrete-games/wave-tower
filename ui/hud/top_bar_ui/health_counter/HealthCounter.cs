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
        get => _health;
        set
        {
            _health = value;
            _counterLabel.Text = value.ToString();
        }
    }

    private int max_health
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            _counterMaxLabel.Text = value.ToString();
        }
    }

    public override void _Ready()
    {
        _counterMaxLabel = GetNode<Label>("CounterMaxLabel");
        _counterLabel = GetNode<Label>("CounterLabel");
        _armorCounter = GetNode<Control>("HBoxContainer/ArmorCounter");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _status = runContext.status;

        _status.health_change += OnHealthChange;
        _status.max_health_change += OnMaxHealthChange;
        _status.armor_change += OnArmorChange;

        max_health = _status.max_health;
        health = _status.health;
        OnArmorChange(_status.armor);
    }

    public override void _ExitTree()
    {
        if (_status != null)
        {
            _status.health_change -= OnHealthChange;
            _status.max_health_change -= OnMaxHealthChange;
            _status.armor_change -= OnArmorChange;
        }
    }

    private void OnHealthChange(int value)
    {
        health = value;
    }

    private void OnMaxHealthChange(int value)
    {
        max_health = value;
    }

    private void OnArmorChange(int amount)
    {
        _armorCounter.Visible = amount > 0;
    }
}
