using Godot;

[GlobalClass]
public partial class RelicsManager : RefCounted
{
    [Signal]
    public delegate void relic_changedEventHandler(string relic_id);

    [Signal]
    public delegate void relic_addedEventHandler(string relic_id);

    [Signal]
    public delegate void relic_removedEventHandler(string relic_id);

    private static readonly Color COMMON_COLOR = Colors.GreenYellow;
    private static readonly Color RARE_COLOR = Colors.DodgerBlue;
    private static readonly Color EPIC_COLOR = Colors.Gold;

    private readonly Godot.Collections.Dictionary<int, Color> _relicColors = new()
    {
        { 0, COMMON_COLOR },
        { 1, RARE_COLOR },
        { 2, EPIC_COLOR },
    };

    private readonly Godot.Collections.Dictionary<string, int> _relicsCount = new();
    private readonly System.Collections.Generic.Dictionary<string, Relic> _relics = new();

    public bool has_relic(string relic_id)
    {
        if (!this._relics.ContainsKey(relic_id))
        {
            return false;
        }

        return !this._relics[relic_id].Disabled;
    }

    public System.Collections.Generic.List<Relic> get_all_relics()
    {
        var values = new System.Collections.Generic.List<Relic>();
        foreach (Relic relic in this._relics.Values)
        {
            values.Add(relic);
        }

        return values;
    }

    public Color get_rarity_color(int rarity)
    {
        if (!this._relicColors.ContainsKey(rarity))
        {
            return COMMON_COLOR;
        }

        return this._relicColors[rarity];
    }

    public void add_relic(Relic relic)
    {
        if (relic == null)
        {
            GD.PushError("[RelicsManager] Attempted to add null relic instance");
            return;
        }

        RelicData dataObj = relic.Data;
        if (dataObj == null)
        {
            GD.PushError("[RelicsManager] Relic data is missing");
            return;
        }

        string relicId = dataObj.id;
        if (this._relics.ContainsKey(relicId))
        {
            GD.PushError($"Relic with ID '{relicId}' already exists. Cannot add duplicate relics.");
            return;
        }

        this.PlayRelicObtain();
        relic.OnObtain();

        if (RunContextRuntime.RelicsManager.GetRelic(relicId) == null)
        {
            RunContextRuntime.RelicsManager.AddRelic(relic);
        }

        // Emit legacy signals only after runtime state is updated so UI recalculations read fresh hooks.
        this._add_relic(relic);
    }

    public void remove_relic(string relic_id)
    {
        if (!this._relics.ContainsKey(relic_id))
        {
            return;
        }

        Relic relicObj = this._relics[relic_id];
        if (relicObj == null)
        {
            this._relics.Remove(relic_id);
            return;
        }

        relicObj.OnRemove();
        relicObj.Changed -= this.emit_relic_changed;
        RunContextRuntime.RelicsManager.RemoveRelic(relic_id);

        this._relics.Remove(relic_id);

        int currentCount = this._relicsCount.ContainsKey(relic_id) ? this._relicsCount[relic_id] : 0;
        this._relicsCount[relic_id] = currentCount - 1;
        this.EmitSignal(SignalName.relic_removed, relic_id);
    }

    private void _add_relic(Relic relic)
    {
        RelicData dataObj = relic.Data;
        if (dataObj == null)
        {
            return;
        }

        string relicId = dataObj.id;

        this._relics[relicId] = relic;
        int currentCount = this._relicsCount.ContainsKey(relicId) ? this._relicsCount[relicId] : 0;
        this._relicsCount[relicId] = currentCount + 1;

        this.EmitSignal(SignalName.relic_added, relicId);
        relic.Changed += this.emit_relic_changed;
    }

    public void emit_relic_changed(Relic relic)
    {
        if (relic == null)
        {
            return;
        }

        this.EmitSignal(SignalName.relic_changed, relic.Id);
    }

    public Relic _get_relic(string id)
    {
        return this._relics.ContainsKey(id) ? this._relics[id] : null;
    }

    private void PlayRelicObtain()
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return;
        }

        Node audioManager = tree.Root.GetNodeOrNull<Node>("/root/AudioManager");
        audioManager?.Call("play_relic_obtain");
    }
}
