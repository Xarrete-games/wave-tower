using Godot;

[GlobalClass]
public partial class LightningChainTower : Tower
{
    [Export] public PackedScene lightning_chain_projectile_scene;

    public int base_bounces = 3;
    public int current_bounces = 3;

    private Marker2D projectile_spawn_point;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_point = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.lightning_chain_projectile_scene == null)
        {
            return;
        }

        Node projectile = this.lightning_chain_projectile_scene.Instantiate();
        CallDeferred(MethodName._fire_chain, projectile);
    }

    private void _fire_chain(Node projectile)
    {
        AddChild(projectile);
        if (projectile is Node2D node2D)
        {
            node2D.GlobalPosition = this.projectile_spawn_point.GlobalPosition;
        }

        projectile.Call("set_target", this._current_target, this._get_attack(), this.current_bounces);
    }
}
