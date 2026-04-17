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
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _status = runContext.status;
        _status.armor_change += OnArmorChange;
        armor = _status.armor;
    }

    public override void _ExitTree()
    {
        if (_status != null)
        {
            _status.armor_change -= OnArmorChange;
        }
    }

    private void OnArmorChange(int value)
    {
        armor = value;
    }
}
