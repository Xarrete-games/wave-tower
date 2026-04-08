public sealed class ConsumableTargeteableModel : ConsumableModel
{
    public enum TargetType
    {
        Tower,
        BlockedTile,
    }

    public TargetType TargetingType { get; set; }
    public TowerModel TargetTower { get; set; }

    public ConsumableTargeteableModel(string id, TargetType targetingType)
        : base(id, ConsumableType.Other)
    {
        this.TargetingType = targetingType;
    }
}
