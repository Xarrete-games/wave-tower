public class CaffeinePotion : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        TowersManager towersManager = runContext?.towers_manager;
        if (towersManager == null)
        {
            return;
        }

        System.Collections.Generic.List<Tower> towers = towersManager.get_placed_towers();
        for (int index = 0; index < towers.Count; index++)
        {
            Tower tower = towers[index];
            if (tower == null)
            {
                continue;
            }

            Source source = get_source();
            TowerBuff debuffObj = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, -20);
            TowerBuff buffObj = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, 20);
            if (debuffObj == null || buffObj == null)
            {
                continue;
            }

            Duration duration = new Duration(5, 0);
            debuffObj.duration = duration;
            buffObj.duration = duration;
            buffObj.residual_buff = debuffObj;

            tower.AddBuff(buffObj);
        }
    }
}
