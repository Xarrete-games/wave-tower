public sealed class TowerBuffModel
{
    public string Id { get; }
    public string SourceId { get; }
    public int Value { get; }

    public TowerBuffModel(string id, string sourceId, int value)
    {
        this.Id = id;
        this.SourceId = sourceId;
        this.Value = value;
    }
}
