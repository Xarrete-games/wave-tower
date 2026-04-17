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
    public bool ApplyBurn { get; set; }
    public int CurrentBounces { get; set; }
    public int ExecuteThreshold { get; set; }
    public float DoubleShotChance { get; set; }

    private readonly System.Collections.Generic.List<TowerBuffModel> _buffs = new();

    public System.Collections.Generic.IReadOnlyList<TowerBuffModel> GetBuffs()
    {
        return _buffs;
    }

    public void AddBuff(TowerBuffModel buff)
    {
        if (buff == null)
        {
            return;
        }

        _buffs.Add(buff);
    }

    public void RemoveBuff(string sourceId)
    {
        if (string.IsNullOrEmpty(sourceId))
        {
            return;
        }

        _buffs.RemoveAll(buff => buff.SourceId == sourceId);
    }
}
