using Godot;

[GlobalClass]
public partial class WildFireTower : Tower
{
    private static readonly Script EnemyDebuffScript = GD.Load<Script>("res://enemies/enemy_debuff/enemy_debuff.gd");

    [Export] public PackedScene projectile_scene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.projectile_scene == null)
        {
            return;
        }

        Node projectile = this.projectile_scene.Instantiate();
        AddChild(projectile);

        if (projectile is Node2D node2D)
        {
            node2D.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;
        }

        projectile.Call("set_target", this._current_target, this._get_attack(), EnemyDebuffScript.Call("create_burn", this.damage_source));
    }
}
