using Godot;

[GlobalClass]
public partial class TestData : Node
{
    [Export]
    public int initial_random_relics = 0;

    [Export]
    public Godot.Collections.Array<string> initial_relics_ids = new();

    [Export]
    public Godot.Collections.Array<string> initial_consumables_ids = new();

    [Export]
    public EventData initial_event;

    [Export]
    public bool open_loot_screen = false;

    private RunHandler _runHandler;

    public async override void _Ready()
    {
        _runHandler = GetNodeOrNull<RunHandler>("../RunHandler");

        await ToSignal(GetTree().CreateTimer(0.1f, false), SceneTreeTimer.SignalName.Timeout);
        HandleInitialRelics();

        if (open_loot_screen && _runHandler != null)
        {
            await _runHandler.ShowLootScreen();
        }

        if (initial_event != null && _runHandler != null)
        {
            await _runHandler.ShowEventsScreen(initial_event);
        }
    }

    private void HandleInitialRelics()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");

        if (initial_relics_ids.Count > 0)
        {
            for (int index = 0; index < initial_relics_ids.Count; index++)
            {
                string relicId = initial_relics_ids[index];
                RelicData typedRelicData = DataLoaderAccess.GetRelicById(relicId);
                if (typedRelicData != null)
                {
                    Relic relicInstance = typedRelicData.CreateItem();
                    runContext.relics_manager.add_relic(relicInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial relic id {relicId} not found");
                }
            }
        }

        if (initial_random_relics > 0)
        {
            System.Collections.Generic.List<RelicData> items = DataLoaderAccess.GetRandomRelicsTyped(initial_random_relics);
            for (int index = 0; index < items.Count; index++)
            {
                RelicData relicDataObj = items[index];
                if (relicDataObj == null)
                {
                    continue;
                }

                Relic relic = relicDataObj?.CreateItem();
                runContext.relics_manager.add_relic(relic);
            }
        }

        if (initial_consumables_ids.Count > 0)
        {
            for (int index = 0; index < initial_consumables_ids.Count; index++)
            {
                string consumableId = initial_consumables_ids[index];
                ConsumableData consumableDataObj = DataLoaderAccess.GetConsumableById(consumableId);
                if (consumableDataObj != null)
                {
                    Consumable consumableInstance = consumableDataObj.CreateConsumable();
                    runContext.consumables_manager.add_consumable(consumableInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial consumable id {consumableId} not found");
                }
            }
        }
    }
}
