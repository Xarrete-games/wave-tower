public sealed class VicMicrophone : TowerBuffRelicBase
{
    private const string _buffId = "attack_range_mult_buff";
    private const int _buffValue = 20;

    public VicMicrophone() : base("vic_microphone")
    {
    }

    protected override void AddBuff(TowerModel tower)
    {
        TowerBuffModel buff = TowerBuffFactoryModel.CreateFromId(_buffId, this.Id, _buffValue);
        if (buff != null)
        {
            tower.AddBuff(buff);
        }
    }

    protected override void RemoveBuff(TowerModel tower)
    {
        tower.RemoveBuff(this.Id);
    }

    protected override bool IsValidTower(TowerModel tower)
    {
        return tower.Type == TowerModel.TowerType.Frost;
    }
}
