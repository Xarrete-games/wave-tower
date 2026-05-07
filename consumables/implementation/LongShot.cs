using Godot;

public class LongShot : ConsumableTargeteable {
    public override void Action(Node target) {
        Tower tower = (Tower)target;
        Source source = GetSource();
        TowerBuff towerBuffObj = TowerBuffFactory.CreateFromId("attack_range_mult_buff", source, 100);
        if (towerBuffObj == null) {
            return;
        }
        towerBuffObj.Duration = new Duration(0, 1);
        tower.AddBuff(towerBuffObj);
    }
}

