public class AttackSpeedMultBuff : TowerBuffStatsModifier {
    public AttackSpeedMultBuff() {
    }
    public AttackSpeedMultBuff(Source source, Duration duration = null, TowerBuff residual_buff = null, BuffData data = null, int value = 0) : base(source, duration, residual_buff, data, value) {
    }
    public override void contribute(TowerStatsAccumulator acc) {
        acc.attack_speed_mult += value / 100.0f;
    }
}

