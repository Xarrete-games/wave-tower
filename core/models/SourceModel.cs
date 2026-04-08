public sealed class SourceModel
{
    public enum SourceType
    {
        Relic,
        Tower,
        Consumable,
        Debuff,
        Global,
    }

    public SourceType Type { get; set; }
    public string TypeId { get; set; } = string.Empty;
    public SourceModel Origin { get; set; }
}
