using Godot;

[GlobalClass]
public partial class LongShot : ConsumableTargeteable
{
    public override void action(Variant p_target)
    {
        GodotObject tower = p_target.AsGodotObject();
        if (tower == null)
        {
            return;
        }

        Source source = this.get_source();
        Variant towerBuff = TowerBuffFactory.create_from_id("attack_range_mult_buff", source, 100);
        TowerBuff towerBuffObj = towerBuff.As<TowerBuff>();
        if (towerBuffObj == null)
        {
            return;
        }

        towerBuffObj.duration = new Duration(0, 1);
        tower.Call("add_buff", towerBuff);
    }
}
