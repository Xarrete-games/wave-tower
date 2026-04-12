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

        Variant source = this.get_source();
        Variant towerBuff = TowerBuffFactory.create_from_id("attack_range_mult_buff", source, 100);
        GodotObject towerBuffObj = towerBuff.AsGodotObject();
        if (towerBuffObj == null)
        {
            return;
        }

        towerBuffObj.Set("duration", new Duration(0, 1));
        tower.Call("add_buff", towerBuff);
    }
}
