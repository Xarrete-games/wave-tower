using Godot;

[GlobalClass]
public partial class FireTower : Tower
{
    [Export] public PackedScene fire_ball_scene;

    public bool apply_burn = false;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.fire_ball_scene == null)
        {
            return;
        }

        Node projectile = this.fire_ball_scene.Instantiate();
        AddChild(projectile);

        if (projectile is Node2D node2D)
        {
            node2D.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;
        }

        GodotObject attack = this._get_attack();
        Variant debuff = this.apply_burn ? EnemyDebuff.create_burn(this.damage_source) : default;
        projectile.Call("set_target", this._current_target, attack, debuff);
    }
}
