public class LongShot : ConsumableTargeteable
{
    public override void action(object p_target)
    {
        Tower tower = p_target as Tower;
        if (tower == null)
        {
            return;
        }

        Source source = this.get_source();
        TowerBuff towerBuffObj = TowerBuffFactory.create_from_id("attack_range_mult_buff", source, 100);
        if (towerBuffObj == null)
        {
            return;
        }

        towerBuffObj.duration = new Duration(0, 1);
        tower.add_buff(towerBuffObj);
    }
}
