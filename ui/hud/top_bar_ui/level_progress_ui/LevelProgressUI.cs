using Godot;

public partial class LevelProgressUI : Control
{
    private static readonly PackedScene LevelProgressSlotScene = GD.Load<PackedScene>("uid://dddlo6uv4ct15");
    private static readonly Texture2D SkullIcon = GD.Load<Texture2D>("uid://drk0ngia6t0aq");
    private static readonly Texture2D QuestionIcon = GD.Load<Texture2D>("uid://ba8nsfyfw75as");
    private static readonly Texture2D RelicIcon = GD.Load<Texture2D>("uid://rvo816wmsjvc");
    private static readonly Texture2D ShopIcon = GD.Load<Texture2D>("uid://cerww538cqrba");

    private static readonly int[] WavesWithEvents = { 8 };
    private static readonly int[] WavesWithShops = { 4 };
    private static readonly int[] WavesWithRelics = { 2, 6 };
    private static readonly int[] WavesWithBoss = { 10 };

    [Export]
    public Control slots_container;

    private RunProgress _progress;

    public override void _Ready()
    {
        this.ClearSlots();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._progress = runContext.progress;
        this._progress.current_wave_changed += this.OnWaveInit;

        ClickEventsBus.ResetGameButtonPressed += this.ClearSlots;
    }

    public override void _ExitTree()
    {
        if (this._progress != null)
        {
            this._progress.current_wave_changed -= this.OnWaveInit;
        }

        ClickEventsBus.ResetGameButtonPressed -= this.ClearSlots;
    }

    private void ClearSlots()
    {
        foreach (Node child in this.slots_container.GetChildren())
        {
            this.slots_container.RemoveChild(child);
            child.QueueFree();
        }

        this.BuildSlots();
    }

    private void BuildSlots()
    {
        for (int index = 0; index < 10; index++)
        {
            LevelProgressSlot slot = LevelProgressSlotScene.Instantiate<LevelProgressSlot>();
            this.slots_container.AddChild(slot);

            int waveNumber = index + 1;
            if (Contains(WavesWithEvents, waveNumber))
            {
                slot.set_icon(QuestionIcon);
            }
            else if (Contains(WavesWithShops, waveNumber))
            {
                slot.set_icon(ShopIcon);
            }
            else if (Contains(WavesWithRelics, waveNumber))
            {
                slot.set_icon(RelicIcon);
            }
            else if (Contains(WavesWithBoss, waveNumber))
            {
                slot.set_icon(SkullIcon);
            }
            else
            {
                slot.set_icon(null);
            }
        }
    }

    private void OnWaveInit(int newValue)
    {
        if (newValue % 10 == 1)
        {
            this.ClearSlots();
        }

        int value = ((newValue - 1) % 10) + 1;
        LevelProgressSlot slot = this.slots_container.GetChild<LevelProgressSlot>(value - 1);
        slot.fill();
    }

    private void _on_mouse_entered()
    {
    }

    private void _on_mouse_exited()
    {
    }

    private static bool Contains(int[] source, int value)
    {
        for (int index = 0; index < source.Length; index++)
        {
            if (source[index] == value)
            {
                return true;
            }
        }

        return false;
    }
}