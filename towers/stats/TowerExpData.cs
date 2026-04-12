using Godot;

[GlobalClass]
public partial class TowerExpData : RefCounted
{
    public int level = 1;
    public int current_exp = 0;
    public int exp_for_next_level = 0;

    public TowerExpData()
    {
    }

    public TowerExpData(int new_level, int new_current_exp, int new_exp_for_next_level)
    {
        this.level = new_level;
        this.current_exp = new_current_exp;
        this.exp_for_next_level = new_exp_for_next_level;
    }
}
