public abstract class TowerBuffStatsModifier : TowerBuff {
    public int Value = 0;
    protected TowerBuffStatsModifier() {
    }
    protected TowerBuffStatsModifier(Source source, Duration duration = null, TowerBuff residualBuff = null, BuffData buffData = null, int buffValue = 0) : base(source, duration, residualBuff, buffData) {
        Value = buffValue;
    }
    public abstract void Contribute(TowerStatsAccumulator acc);
}

