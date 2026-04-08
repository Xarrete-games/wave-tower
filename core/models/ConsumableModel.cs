public abstract class ConsumableModel
{
    public enum ConsumableType
    {
        Other,
        Potion,
    }

    public string Id { get; }
    public ConsumableType Type { get; }

    protected ConsumableModel(string id, ConsumableType type)
    {
        this.Id = id;
        this.Type = type;
    }
}
