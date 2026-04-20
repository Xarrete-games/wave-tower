using Godot;
using System;

public class RelicsManager
{
    public event Action<string> RelicChanged;
    public event Action<string> RelicAdded;
    public event Action<string> RelicRemoved;

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

    public bool HasRelic(string relicId)
    {
        if (!_relics.ContainsKey(relicId))
        {
            return false;
        }

        return !_relics[relicId].Disabled;
    }

    public System.Collections.Generic.List<Relic> GetAllRelics()
    {
        var values = new System.Collections.Generic.List<Relic>();
        foreach (Relic relic in _relics.Values)
        {
            values.Add(relic);
        }

        return values;
    }

    public Color GetRarityColor(int rarity)
    {
        if (!_relicColors.ContainsKey(rarity))
        {
            return COMMON_COLOR;
        }

        return _relicColors[rarity];
    }

    public void AddRelic(Relic relic)
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

        string relicId = dataObj.Id;
        if (_relics.ContainsKey(relicId))
        {
            GD.PushError($"Relic with ID '{relicId}' already exists. Cannot add duplicate relics.");
            return;
        }

        PlayRelicObtain();
        relic.OnObtain();

        if (RunContextRuntime.RelicsManager.GetRelic(relicId) == null)
        {
            RunContextRuntime.RelicsManager.AddRelic(relic);
        }

        // Emit legacy signals only after runtime state is updated so UI recalculations read fresh hooks.
        AddRelicInternal(relic);
    }

    public void RemoveRelic(string relicId)
    {
        if (!_relics.ContainsKey(relicId))
        {
            return;
        }

        Relic relicObj = _relics[relicId];
        if (relicObj == null)
        {
            _relics.Remove(relicId);
            return;
        }

        relicObj.OnRemove();
        relicObj.Changed -= EmitRelicChanged;
        RunContextRuntime.RelicsManager.RemoveRelic(relicId);

        _relics.Remove(relicId);

        int currentCount = _relicsCount.ContainsKey(relicId) ? _relicsCount[relicId] : 0;
        _relicsCount[relicId] = currentCount - 1;
        RelicRemoved?.Invoke(relicId);
    }

    private void AddRelicInternal(Relic relic)
    {
        RelicData dataObj = relic.Data;
        if (dataObj == null)
        {
            return;
        }

        string relicId = dataObj.Id;

        _relics[relicId] = relic;
        int currentCount = _relicsCount.ContainsKey(relicId) ? _relicsCount[relicId] : 0;
        _relicsCount[relicId] = currentCount + 1;

        RelicAdded?.Invoke(relicId);
        relic.Changed += EmitRelicChanged;
    }

    public void EmitRelicChanged(Relic relic)
    {
        if (relic == null)
        {
            return;
        }

        RelicChanged?.Invoke(relic.Id);
    }

    public Relic GetRelic(string id)
    {
        return _relics.ContainsKey(id) ? _relics[id] : null;
    }

    private void PlayRelicObtain()
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return;
        }

        AudioManager audioManager = tree.Root.GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.PlayRelicObtain();
    }
}
