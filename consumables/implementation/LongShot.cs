public class LongShot : ConsumableTargeteable {
    public override void Action(object target) {
        Tower tower = target as Tower;
        if (tower == null) {
            return;
        }
        Source source = GetSource();
        TowerBuff towerBuffObj = TowerBuffFactory.CreateFromId("attack_range_mult_buff", source, 100);
        if (towerBuffObj == null) {
            return;
        }
        towerBuffObj.Duration = new Duration(0, 1);
        tower.AddBuff(towerBuffObj);
    }
}

