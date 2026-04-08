public sealed class Metronome : RelicModel
{
    private const string _buffId = "attack_speed_mult_buff";
    private const int _buffValue = 5;
    private const int _counterThreshold = 3;

    public Metronome() : base("metronome")
    {
    }

    public override void OnWaveFinished()
    {
        this.Counter += 1;
        if (this.Counter < _counterThreshold)
        {
            return;
        }

        this.Counter = 0;
        TowerModel tower = RunContextRuntime.TowersManager.PickRandomTower();
        if (tower == null)
        {
            return;
        }

        TowerBuffModel buff = TowerBuffFactoryModel.CreateFromId(_buffId, this.Id, _buffValue);
        if (buff != null)
        {
            tower.AddBuff(buff);
        }
    }
}
