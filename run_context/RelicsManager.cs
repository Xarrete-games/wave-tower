using Godot;

[GlobalClass]
public partial class RelicsManager : RefCounted
{
    [Signal]
    public delegate void relic_changedEventHandler(Variant relic);

    [Signal]
    public delegate void relic_addedEventHandler(Variant relic);

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
    private readonly Godot.Collections.Dictionary<string, Variant> _relics = new();

    public bool has_relic(string relic_id)
    {
        if (!this._relics.ContainsKey(relic_id))
        {
            return false;
        }

        GodotObject relicObj = this._relics[relic_id].AsGodotObject();
        if (relicObj == null)
        {
            return false;
        }

        return !(bool)relicObj.Get("disabled");
    }

    public Godot.Collections.Array<Variant> get_all_relics()
    {
        var values = new Godot.Collections.Array<Variant>();
        foreach (Variant relic in this._relics.Values)
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

    public void add_relic(Variant relic)
    {
        if (relic.VariantType == Variant.Type.Nil)
        {
            GD.PushError("[RelicsManager] Attempted to add null relic instance");
            return;
        }

        RelicRuntimeAdapter relicObj = relic.AsGodotObject() as RelicRuntimeAdapter;
        if (relicObj == null)
        {
            GD.PushError("[RelicsManager] Attempted to add invalid relic instance");
            return;
        }

        RelicData dataObj = relicObj.data.As<RelicData>();
        if (dataObj == null)
        {
            GD.PushError("[RelicsManager] Relic data is invalid");
            return;
        }

        string relicId = dataObj.id;
        if (this._relics.ContainsKey(relicId))
        {
            GD.PushError($"Relic with ID '{relicId}' already exists. Cannot add duplicate relics.");
            return;
        }

        this.PlayRelicObtain();
        relicObj.on_obtain();
        this._add_relic(relic);

        RunContextRuntime.RelicsManager.AddRelicById(relicId);
    }

    public void remove_relic(string relic_id)
    {
        if (!this._relics.ContainsKey(relic_id))
        {
            return;
        }

        RelicRuntimeAdapter relicObj = this._relics[relic_id].AsGodotObject() as RelicRuntimeAdapter;
        if (relicObj == null)
        {
            this._relics.Remove(relic_id);
            return;
        }

        relicObj.on_remove();
        relicObj.changed -= this.emit_relic_changed;
        this._relics.Remove(relic_id);

        int currentCount = this._relicsCount.ContainsKey(relic_id) ? this._relicsCount[relic_id] : 0;
        this._relicsCount[relic_id] = currentCount - 1;
        this.EmitSignal(SignalName.relic_removed, relic_id);

        RunContextRuntime.RelicsManager.RemoveRelic(relic_id);
    }

    private void _add_relic(Variant relic)
    {
        RelicRuntimeAdapter relicObj = relic.AsGodotObject() as RelicRuntimeAdapter;
        if (relicObj == null)
        {
            return;
        }

        RelicData dataObj = relicObj.data.As<RelicData>();
        if (dataObj == null)
        {
            return;
        }

        string relicId = dataObj.id;

        this._relics[relicId] = relic;
        int currentCount = this._relicsCount.ContainsKey(relicId) ? this._relicsCount[relicId] : 0;
        this._relicsCount[relicId] = currentCount + 1;

        this.EmitSignal(SignalName.relic_added, relic);
        relicObj.changed += this.emit_relic_changed;
    }

    public void emit_relic_changed(Variant relic)
    {
        this.EmitSignal(SignalName.relic_changed, relic);
    }

    public Variant _get_relic(string id)
    {
        return this._relics.ContainsKey(id) ? this._relics[id] : default;
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
