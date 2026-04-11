using Godot;
using System.Threading.Tasks;

public partial class RunHandler : Node
{
    private static readonly PackedScene NextWaveScreenScene = GD.Load<PackedScene>("uid://b7ttkk4pasgin");
    private static readonly PackedScene NextLevelScreenScene = GD.Load<PackedScene>("uid://crastw7xnqgvl");
    private static readonly PackedScene EndGameScene = GD.Load<PackedScene>("uid://ovtc0l4cimpl");
    private static readonly PackedScene ChooseTowerScreenScene = GD.Load<PackedScene>("uid://bxw2isn60dlre");

    private static readonly int[] WavesWithEvents = { 8 };
    private static readonly int[] WavesWithShops = { 4 };
    private static readonly int[] WavesWithRelics = { 2, 6, 10 };

    [Export]
    public CanvasLayer event_layer;

    [Export]
    public EventsScreenHandler events_screen_hander;

    [Export]
    public LootScreenHandler loot_screen_handler;

    private Godot.Collections.Array<Variant> _events = new();
    private GodotObject _shopEvent;
    private GodotObject _chooseRelicEvent;
    private Godot.Collections.Array<Variant> _optionsEvents = new();

    public override void _Ready()
    {
        this.SetEventsByType();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.Connect("current_wave_finished", Callable.From(this.OnWaveFinished));
        runContext.progress.Connect("last_wave_finished", Callable.From(this.OnLastWaveFinished));

        this.ShowNextWaveScreen();
    }

    public async Task ShowLootScreen()
    {
        if (this.loot_screen_handler == null)
        {
            GD.PushError("[RunHandler] loot_screen_handler is null.");
            return;
        }

        await this.loot_screen_handler.ShowLootScreenAsync(this.event_layer);
    }

    public async Task ShowChooseCardScreen()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        var cards = runContext.towers_manager.get_random_towers(3);

        ChooseTowerScreen chooseTowerScreen = ChooseTowerScreenScene.Instantiate<ChooseTowerScreen>();
        this.event_layer.AddChild(chooseTowerScreen);
        chooseTowerScreen.PopulateScreen(cards);

        await ToSignal(chooseTowerScreen, "done");
    }

    public async Task ShowEventsScreen(GodotObject eventData)
    {
        if (this.events_screen_hander == null || eventData == null)
        {
            return;
        }

        await this.events_screen_hander.ShowEventSelectedAsync(eventData, this.event_layer);
    }

    private void SetEventsByType()
    {
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        this._events = dataLoader.get_all_events();
        this._shopEvent = null;
        this._chooseRelicEvent = null;
        this._optionsEvents = new Godot.Collections.Array<Variant>();

        foreach (Variant eventVariant in this._events)
        {
            GodotObject eventData = eventVariant.AsGodotObject();
            if (eventData == null)
            {
                continue;
            }

            int eventType = (int)eventData.Get("type");
            if (eventType == 1)
            {
                this._shopEvent = eventData;
            }
            else if (eventType == 2)
            {
                this._chooseRelicEvent = eventData;
            }
            else if (eventType == 0)
            {
                this._optionsEvents.Add(eventData);
            }
        }
    }

    private void ShowNextWaveScreen()
    {
        Node nextWaveScreen = NextWaveScreenScene.Instantiate();
        this.event_layer.CallDeferred("add_child", nextWaveScreen);
    }

    private void ShowNextLevelMenu()
    {
        Node nextLevelScreen = NextLevelScreenScene.Instantiate();
        this.event_layer.CallDeferred("add_child", nextLevelScreen);
    }

    private async void OnWaveFinished()
    {
        GetNode<Node>("/root/AudioManager").Call("play_wave_clear");
        await this.ShowLootScreen();
        await this.ShowChooseCardScreen();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (runContext.is_on_restarting)
        {
            return;
        }

        int currentWave = (int)runContext.progress.Get("current_wave");
        GodotObject eventData = this.GetNextEvent(currentWave);
        if (eventData == null)
        {
            this.ShowNextWaveScreen();
            return;
        }

        await this.ShowEventsScreen(eventData);
        this.ShowNextWaveScreen();
    }

    private async void OnLastWaveFinished()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int currentGold = (int)runContext.economy.Get("gold");
        runContext.economy.Set("gold", currentGold + 50);

        bool isLastWave = (bool)runContext.progress.Call("is_last_wave");
        if (isLastWave)
        {
            await ToSignal(GetTree().CreateTimer(5, false), "timeout");
            GetTree().ChangeSceneToPacked(EndGameScene);
            return;
        }

        this.ShowNextLevelMenu();
    }

    private GodotObject GetNextEvent(int currentWave)
    {
        int waveInCycle = ((currentWave - 1) % 10) + 1;

        if (Contains(WavesWithShops, waveInCycle) && this._shopEvent != null)
        {
            return this._shopEvent;
        }

        if (Contains(WavesWithRelics, waveInCycle) && this._chooseRelicEvent != null)
        {
            return this._chooseRelicEvent;
        }

        if (Contains(WavesWithEvents, waveInCycle) && this._optionsEvents.Count > 0)
        {
            this._optionsEvents.Shuffle();
            Variant picked = this._optionsEvents[this._optionsEvents.Count - 1];
            this._optionsEvents.RemoveAt(this._optionsEvents.Count - 1);
            return picked.AsGodotObject();
        }

        return null;
    }

    private static bool Contains(int[] source, int value)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] == value)
            {
                return true;
            }
        }

        return false;
    }
}
