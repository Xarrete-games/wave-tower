public sealed class ValveAmplifier : Relic
{
    private const string _buffId = "attack_range_mult_buff";
    private const int _buffValue = 10;
    private const int _counterThreshold = 3;

    public ValveAmplifier() : base("valve_amplifier")
    {
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        this.Counter += 1;
        if (this.Counter < _counterThreshold)
        {
            return;
        }

        this.Counter = 0;
        TowerBuffModel buff = TowerBuffFactoryModel.CreateFromId(_buffId, this.Id, _buffValue);
        if (buff != null)
        {
            tower.AddBuff(buff);
        }
    }
}
