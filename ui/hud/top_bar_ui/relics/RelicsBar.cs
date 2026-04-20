using Godot;

public partial class RelicsBar : Control
{
    private static readonly PackedScene TopBarRelic = GD.Load<PackedScene>("uid://f34dinc60kaa");
    private RelicsManager _relicsManager;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _relicsManager = runContext.relics_manager;
        if (_relicsManager != null)
        {
            _relicsManager.RelicAdded += OnRelicAdded;
            _relicsManager.RelicRemoved += OnRelicRemoved;
            _relicsManager.RelicChanged += OnRelicChanged;
        }
    }

    public override void _ExitTree()
    {
        if (_relicsManager != null)
        {
            _relicsManager.RelicAdded -= OnRelicAdded;
            _relicsManager.RelicRemoved -= OnRelicRemoved;
            _relicsManager.RelicChanged -= OnRelicChanged;
            _relicsManager = null;
        }
    }

    private void OnRelicAdded(string relicId)
    {
        Relic relic = _relicsManager?.GetRelic(relicId);
        if (relic == null)
        {
            return;
        }

        RelicUI relicInstance = TopBarRelic.Instantiate<RelicUI>();
        AddChild(relicInstance);
        relicInstance.SetRelic(relic);
    }

    private void OnRelicChanged(string relicId)
    {
        Relic relic = _relicsManager?.GetRelic(relicId);
        if (relic == null)
        {
            return;
        }

        foreach (Node child in GetChildren())
        {
            RelicUI relicUi = child as RelicUI;
            if (relicUi?.Relic?.Id == relicId)
            {
                relicUi.SetRelic(relic);
            }
        }
    }

    private void OnRelicRemoved(string relicId)
    {
        foreach (Node child in GetChildren())
        {
            RelicUI relicUi = child as RelicUI;
            if (relicUi?.Relic?.Id == relicId)
            {
                relicUi.QueueFree();
            }
        }
    }
}