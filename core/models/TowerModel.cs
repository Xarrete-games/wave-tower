public sealed class TowerModel
{
    public string Id { get; set; } = string.Empty;
    public TowerTargetingMode TargetingMode { get; set; } = TowerTargetingMode.FirstInProgress;
}
