using Godot;

public partial class RelicsBar : Control
{
    private static readonly PackedScene TopBarRelic = GD.Load<PackedScene>("uid://f34dinc60kaa");

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.relics_manager.Connect("relic_added", Callable.From<Variant>(this.OnRelicAdded));
        runContext.relics_manager.Connect("relic_removed", Callable.From<string>(this.OnRelicRemoved));
        runContext.relics_manager.Connect("relic_changed", Callable.From<Variant>(this.OnRelicChanged));
    }

    private void OnRelicAdded(Variant relic)
    {
        RelicUI relicInstance = TopBarRelic.Instantiate<RelicUI>();
        AddChild(relicInstance);
        relicInstance.SetRelic(relic);
    }

    private void OnRelicChanged(Variant relic)
    {
        GodotObject relicObj = relic.AsGodotObject();
        GodotObject relicData = relicObj?.Get("data").AsGodotObject();
        if (relicData == null)
        {
            return;
        }

        string relicId = (string)relicData.Get("id");
        foreach (Node child in GetChildren())
        {
            RelicUI relicUi = child as RelicUI;
            GodotObject existingRelicObj = relicUi?.relic.AsGodotObject();
            GodotObject existingData = existingRelicObj?.Get("data").AsGodotObject();
            if (existingData != null && (string)existingData.Get("id") == relicId)
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
            GodotObject relicObj = relicUi?.relic.AsGodotObject();
            GodotObject data = relicObj?.Get("data").AsGodotObject();
            if (data != null && (string)data.Get("id") == relicId)
            {
                relicUi.QueueFree();
            }
        }
    }
}