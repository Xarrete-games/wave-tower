using Godot;

[GlobalClass]
public partial class EnemyManager : RefCounted
{
    [Signal]
    public delegate void enemy_dieEventHandler(Variant enemy, Variant attack);

    [Signal]
    public delegate void enemy_target_reachedEventHandler(Variant enemy);
}
