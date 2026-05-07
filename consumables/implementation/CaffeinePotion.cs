public class CaffeinePotion : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = RunContext.Instance;
        TowersManager towersManager = runContext.TowersManager;

        System.Collections.Generic.List<Tower> towers = towersManager.GetPlacedTowers();
        for (int index = 0; index < towers.Count; index++)
        {
            Tower tower = towers[index];
            if (tower == null)
            {
                continue;
            }

            Source source = GetSource();
            TowerBuff debuffObj = TowerBuffFactory.CreateFromId("attack_speed_mult_buff", source, -20);
            TowerBuff buffObj = TowerBuffFactory.CreateFromId("attack_speed_mult_buff", source, 20);
            if (debuffObj == null || buffObj == null)
            {
                continue;
            }

            Duration duration = new Duration(5, 0);
            debuffObj.Duration = duration;
            buffObj.Duration = duration;
            buffObj.ResidualBuff = debuffObj;

            tower.AddBuff(buffObj);
        }
    }
}
