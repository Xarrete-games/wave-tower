public class DamageFlatBuff : TowerBuffStatsModifier {
    public DamageFlatBuff() {
    }
    public DamageFlatBuff(Source source, Duration duration = null, TowerBuff residual_buff = null, BuffData data = null, int value = 0) : base(source, duration, residual_buff, data, value) {
    }
    public override void contribute(TowerStatsAccumulator acc) {
        acc.flat_damage += value;
    }
}

