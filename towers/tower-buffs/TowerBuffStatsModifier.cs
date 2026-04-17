public abstract class TowerBuffStatsModifier : TowerBuff {
    public int value = 0;
    protected TowerBuffStatsModifier() {
    }
    protected TowerBuffStatsModifier(Source source, Duration duration = null, TowerBuff residual_buff = null, BuffData data = null, int buffValue = 0) : base(source, duration, residual_buff, data) {
        value = buffValue;
    }
    public abstract void contribute(TowerStatsAccumulator acc);
}

