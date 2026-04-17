public class AttackRangeMultBuff : TowerBuffStatsModifier {
    public AttackRangeMultBuff() {
    }
    public AttackRangeMultBuff(Source source, Duration duration = null, TowerBuff residual_buff = null, BuffData data = null, int value = 0) : base(source, duration, residual_buff, data, value) {
    }
    public override void contribute(TowerStatsAccumulator acc) {
        acc.attack_range_mult += value / 100.0f;
    }
}

