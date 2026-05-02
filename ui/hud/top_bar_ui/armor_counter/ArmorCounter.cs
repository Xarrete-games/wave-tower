using Godot;

[GlobalClass]
public partial class ArmorCounter : CenterContainer
{
    private Label _counterLabel;
    private Status _status;
    private int _armor;

    public int armor
    {
        get => _armor;
        set
        {
            _armor = value;
            if (_counterLabel != null)
            {
                _counterLabel.Text = value.ToString();
            }
        }
    }

    public override void _Ready()
    {
        _counterLabel = GetNode<Label>("CounterLabel");
        RunContext runContext = RunContext.Instance;
        _status = runContext.Status;
        _status.ArmorChanged += OnArmorChange;
        armor = _status.Armor;
    }

    public override void _ExitTree()
    {
        if (_status != null)
        {
            _status.ArmorChanged -= OnArmorChange;
        }
    }

    private void OnArmorChange(int value)
    {
        armor = value;
    }
}
