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
            _relicsManager.relic_added += OnRelicAdded;
            _relicsManager.relic_removed += OnRelicRemoved;
            _relicsManager.relic_changed += OnRelicChanged;
        }
    }

    public override void _ExitTree()
    {
        if (_relicsManager != null)
        {
            _relicsManager.relic_added -= OnRelicAdded;
            _relicsManager.relic_removed -= OnRelicRemoved;
            _relicsManager.relic_changed -= OnRelicChanged;
            _relicsManager = null;
        }
    }

    private void OnRelicAdded(string relicId)
    {
        Relic relic = _relicsManager?._get_relic(relicId);
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
        Relic relic = _relicsManager?._get_relic(relicId);
        if (relic == null)
        {
            return;
        }

        foreach (Node child in GetChildren())
        {
            RelicUI relicUi = child as RelicUI;
            if (relicUi?.relic?.Id == relicId)
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
            if (relicUi?.relic?.Id == relicId)
            {
                relicUi.QueueFree();
            }
        }
    }
}