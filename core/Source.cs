public sealed record Source(Source.SourceType Type, string TypeId, object Entity = null, Source Origin = null)
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
