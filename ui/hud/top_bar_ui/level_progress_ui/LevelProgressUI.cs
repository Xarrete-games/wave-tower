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
    public Control SlotsContainer;

    private RunProgress _progress;

    public override void _Ready()
    {
        ClearSlots();

        RunContext runContext = RunContext.Instance;
        _progress = runContext.Progress;
        _progress.CurrentWaveChanged += OnWaveInit;

        ClickEvents.ResetGameButtonPressed += ClearSlots;
    }

    public override void _ExitTree()
    {
        if (_progress != null)
        {
            _progress.CurrentWaveChanged -= OnWaveInit;
        }

        ClickEvents.ResetGameButtonPressed -= ClearSlots;
    }

    private void ClearSlots()
    {
        foreach (Node child in SlotsContainer.GetChildren())
        {
            SlotsContainer.RemoveChild(child);
            child.QueueFree();
        }

        BuildSlots();
    }

    private void BuildSlots()
    {
        for (int index = 0; index < 10; index++)
        {
            LevelProgressSlot slot = LevelProgressSlotScene.Instantiate<LevelProgressSlot>();
            SlotsContainer.AddChild(slot);

            int waveNumber = index + 1;
            if (Contains(WavesWithEvents, waveNumber))
            {
                slot.SetIcon(QuestionIcon);
            }
            else if (Contains(WavesWithShops, waveNumber))
            {
                slot.SetIcon(ShopIcon);
            }
            else if (Contains(WavesWithRelics, waveNumber))
            {
                slot.SetIcon(RelicIcon);
            }
            else if (Contains(WavesWithBoss, waveNumber))
            {
                slot.SetIcon(SkullIcon);
            }
            else
            {
                slot.SetIcon(null);
            }
        }
    }

    private void OnWaveInit(int newValue)
    {
        if (newValue % 10 == 1)
        {
            ClearSlots();
        }

        int value = ((newValue - 1) % 10) + 1;
        LevelProgressSlot slot = SlotsContainer.GetChild<LevelProgressSlot>(value - 1);
        slot.Fill();
    }

    private void OnMouseEntered()
    {
    }

    private void OnMouseExited()
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
