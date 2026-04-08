public sealed class TowerModel
{
    public enum TowerType
    {
        Fire,
        Lightning,
        Frost,
    }

    public string Id { get; set; } = string.Empty;
    public string TypeId { get; set; } = string.Empty;
    public TowerType Type { get; set; } = TowerType.Fire;
    public TowerTargetingMode TargetingMode { get; set; } = TowerTargetingMode.FirstInProgress;
}
