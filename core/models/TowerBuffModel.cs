public sealed class TowerBuffModel
{
    public string Id { get; }
    public string SourceId { get; }
    public int Value { get; }

    public TowerBuffModel(string id, string sourceId, int value)
    {
        Id = id;
        SourceId = sourceId;
        Value = value;
    }
}
