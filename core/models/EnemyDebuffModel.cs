public sealed class EnemyDebuffModel
{
    public enum DebuffType
    {
        Frost,
        Burn,
    }

    public string Id { get; set; } = string.Empty;
    public DebuffType Type { get; set; }
    public float Value { get; set; }
    public float Duration { get; set; }
    public float TickInterval { get; set; }
    public int MaxStacks { get; set; } = 99;
}
