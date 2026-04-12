using Godot;

[GlobalClass]
public partial class LightningTower : Tower
{
    [Export] public PackedScene electric_ball_scene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.electric_ball_scene == null)
        {
            return;
        }

        Node projectile = this.electric_ball_scene.Instantiate();
        AddChild(projectile);

        if (projectile is Node2D node2D)
        {
            node2D.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;
        }

        projectile.Call("set_target", this._current_target, this._get_attack());
    }
}
