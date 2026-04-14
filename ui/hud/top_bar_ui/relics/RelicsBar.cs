using Godot;

public partial class RelicsBar : Control
{
    private static readonly PackedScene TopBarRelic = GD.Load<PackedScene>("uid://f34dinc60kaa");
    private RelicsManager _relicsManager;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._relicsManager = runContext.relics_manager;
        if (this._relicsManager != null)
        {
            this._relicsManager.relic_added += this.OnRelicAdded;
            this._relicsManager.relic_removed += this.OnRelicRemoved;
            this._relicsManager.relic_changed += this.OnRelicChanged;
        }
    }

    public override void _ExitTree()
    {
        if (this._relicsManager != null)
        {
            this._relicsManager.relic_added -= this.OnRelicAdded;
            this._relicsManager.relic_removed -= this.OnRelicRemoved;
            this._relicsManager.relic_changed -= this.OnRelicChanged;
            this._relicsManager = null;
        }
    }

    private void OnRelicAdded(string relicId)
    {
        Relic relic = this._relicsManager?._get_relic(relicId);
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
        Relic relic = this._relicsManager?._get_relic(relicId);
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