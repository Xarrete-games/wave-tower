public sealed record Source(Source.SourceType Type, string TypeId, Source Origin = null)
{
    public enum SourceType
    {
        RELIC,
        TOWER,
        CONSUMABLE,
        DEBUFF,
        GLOBAL,
    }
}
