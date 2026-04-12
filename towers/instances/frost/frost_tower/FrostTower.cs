using Godot;

[GlobalClass]
public partial class FrostTower : Tower
{
    private static readonly PackedScene FrostBallScene = GD.Load<PackedScene>("uid://cibktj8x8j1t8");
    private static readonly Script EnemyDebuffScript = GD.Load<Script>("res://enemies/enemy_debuff/enemy_debuff.gd");

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target))
        {
            return;
        }

        Node projectile = FrostBallScene.Instantiate();
        AddChild(projectile);

        if (projectile is Node2D node2D)
        {
            node2D.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;
        }

        GodotObject attack = this._get_attack();
        Variant debuff = EnemyDebuffScript.Call("create_frost", this.damage_source);
        projectile.Call("set_target", this._current_target, attack, debuff);
    }
}
