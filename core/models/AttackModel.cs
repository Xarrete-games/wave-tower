public sealed class AttackModel
{
    public float Damage { get; set; }
    public float CritChance { get; set; }
    public bool IsCritical { get; set; }
    public bool IsExecution { get; set; }
    public int Hits { get; set; } = 1;
    public int Bounces { get; set; }
    public SourceModel Source { get; set; } = new SourceModel();
}
