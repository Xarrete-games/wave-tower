using Godot;

[GlobalClass]
public partial class TestData : Node
{
    [Export]
    public int InitialRandomRelics = 0;

    [Export]
    public Godot.Collections.Array<string> InitialRelicsIds = new();

    [Export]
    public Godot.Collections.Array<string> InitialConsumablesIds = new();

    [Export]
    public EventData InitialEvent;

    [Export]
    public bool OpenLootScreen = false;

    private RunHandler _runHandler;

    public async override void _Ready()
    {
        _runHandler = GetNodeOrNull<RunHandler>("../RunHandler");

        await ToSignal(GetTree().CreateTimer(0.1f, false), SceneTreeTimer.SignalName.Timeout);
        HandleInitialRelics();

        if (OpenLootScreen && _runHandler != null)
        {
            await _runHandler.ShowLootScreen();
        }

        if (InitialEvent != null && _runHandler != null)
        {
            await _runHandler.ShowEventsScreen(InitialEvent);
        }
    }

    private void HandleInitialRelics()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");

        if (InitialRelicsIds.Count > 0)
        {
            for (int index = 0; index < InitialRelicsIds.Count; index++)
            {
                string relicId = InitialRelicsIds[index];
                RelicData typedRelicData = DataLoaderAccess.GetRelicById(relicId);
                if (typedRelicData != null)
                {
                    Relic relicInstance = typedRelicData.CreateItem();
                    runContext.RelicsManager.AddRelic(relicInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial relic id {relicId} not found");
                }
            }
        }

        if (InitialRandomRelics > 0)
        {
            System.Collections.Generic.List<RelicData> items = DataLoaderAccess.GetRandomRelicsTyped(InitialRandomRelics);
            for (int index = 0; index < items.Count; index++)
            {
                RelicData relicDataObj = items[index];
                if (relicDataObj == null)
                {
                    continue;
                }

                Relic relic = relicDataObj?.CreateItem();
                runContext.RelicsManager.AddRelic(relic);
            }
        }

        if (InitialConsumablesIds.Count > 0)
        {
            for (int index = 0; index < InitialConsumablesIds.Count; index++)
            {
                string consumableId = InitialConsumablesIds[index];
                ConsumableData consumableDataObj = DataLoaderAccess.GetConsumableById(consumableId);
                if (consumableDataObj != null)
                {
                    Consumable consumableInstance = consumableDataObj.CreateConsumable();
                    runContext.ConsumablesManager.AddConsumable(consumableInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial consumable id {consumableId} not found");
                }
            }
        }
    }
}
