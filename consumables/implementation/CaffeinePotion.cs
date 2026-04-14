using Godot;

[GlobalClass]
public partial class CaffeinePotion : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        TowersManager towersManager = runContext?.towers_manager;
        if (towersManager == null)
        {
            return;
        }

        Godot.Collections.Array<Variant> towers = towersManager.towers;
        for (int index = 0; index < towers.Count; index++)
        {
            GodotObject tower = towers[index].AsGodotObject();
            if (tower == null)
            {
                continue;
            }

            Source source = this.get_source();
            Variant debuff = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, -20);
            Variant buff = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, 20);

            TowerBuff debuffObj = debuff.As<TowerBuff>();
            TowerBuff buffObj = buff.As<TowerBuff>();
            if (debuffObj == null || buffObj == null)
            {
                continue;
            }

            Duration duration = new Duration(5, 0);
            debuffObj.duration = duration;
            buffObj.duration = duration;
            buffObj.residual_buff = debuffObj;

            tower.Call("add_buff", buff);
        }
    }
}
