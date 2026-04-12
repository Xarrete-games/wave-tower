using Godot;

[GlobalClass]
public partial class CaffeinePotion : ConsumableUsable
{
    public override void use()
    {
        GodotObject towersManager = this.GetSingleton("RunContext")?.Get("towers_manager").AsGodotObject();
        if (towersManager == null)
        {
            return;
        }

        Godot.Collections.Array<Variant> towers = towersManager.Get("towers").AsGodotArray<Variant>();
        for (int index = 0; index < towers.Count; index++)
        {
            GodotObject tower = towers[index].AsGodotObject();
            if (tower == null)
            {
                continue;
            }

            Variant source = this.get_source();
            Variant debuff = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, -20);
            Variant buff = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, 20);

            GodotObject debuffObj = debuff.AsGodotObject();
            GodotObject buffObj = buff.AsGodotObject();
            if (debuffObj == null || buffObj == null)
            {
                continue;
            }

            Variant duration = new Duration(5, 0);
            debuffObj.Set("duration", duration);
            buffObj.Set("duration", duration);
            buffObj.Set("residual_buff", debuff);

            tower.Call("add_buff", buff);
        }
    }
}
