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
        this._runHandler = GetNodeOrNull<RunHandler>("../RunHandler");

        await ToSignal(GetTree().CreateTimer(0.1f, false), SceneTreeTimer.SignalName.Timeout);
        this._handle_initial_relics();

        if (this.open_loot_screen && this._runHandler != null)
        {
            await this._runHandler.ShowLootScreen();
        }

        if (this.initial_event != null && this._runHandler != null)
        {
            await this._runHandler.ShowEventsScreen(this.initial_event);
        }
    }

    private void _handle_initial_relics()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");

        if (this.initial_relics_ids.Count > 0)
        {
            for (int index = 0; index < this.initial_relics_ids.Count; index++)
            {
                string relicId = this.initial_relics_ids[index];
                RelicData typedRelicData = DataLoaderAccess.GetRelicById(relicId);
                if (typedRelicData != null)
                {
                    Relic relicInstance = typedRelicData.create_item();
                    runContext.relics_manager.add_relic(relicInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial relic id {relicId} not found");
                }
            }
        }

        if (this.initial_random_relics > 0)
        {
            System.Collections.Generic.List<RelicData> items = DataLoaderAccess.GetRandomRelicsTyped(this.initial_random_relics);
            for (int index = 0; index < items.Count; index++)
            {
                RelicData relicDataObj = items[index];
                if (relicDataObj == null)
                {
                    continue;
                }

                Relic relic = relicDataObj?.create_item();
                runContext.relics_manager.add_relic(relic);
            }
        }

        if (this.initial_consumables_ids.Count > 0)
        {
            for (int index = 0; index < this.initial_consumables_ids.Count; index++)
            {
                string consumableId = this.initial_consumables_ids[index];
                ConsumableData consumableDataObj = DataLoaderAccess.GetConsumableById(consumableId);
                if (consumableDataObj != null)
                {
                    Consumable consumableInstance = consumableDataObj.create_consumable();
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
