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

    private int MaxHealth
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
        _status = runContext.Status;

        _status.HealthChanged += OnHealthChange;
        _status.MaxHealthChanged += OnMaxHealthChange;
        _status.ArmorChanged += OnArmorChange;

        MaxHealth = _status.MaxHealth;
        health = _status.Health;
        OnArmorChange(_status.Armor);
    }

    public override void _ExitTree()
    {
        if (_status != null)
        {
            _status.HealthChanged -= OnHealthChange;
            _status.MaxHealthChanged -= OnMaxHealthChange;
            _status.ArmorChanged -= OnArmorChange;
        }
    }

    private void OnHealthChange(int value)
    {
        health = value;
    }

    private void OnMaxHealthChange(int value)
    {
        MaxHealth = value;
    }

    private void OnArmorChange(int amount)
    {
        _armorCounter.Visible = amount > 0;
    }
}

