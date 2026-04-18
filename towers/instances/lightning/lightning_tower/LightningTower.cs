using Godot;

[GlobalClass]
public partial class LightningTower : Tower
{
    [Export] public PackedScene ElectricBallScene;

    private Marker2D _projectileSpawnPos;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || ElectricBallScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = ElectricBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = _projectileSpawnPos.GlobalPosition;

        projectile.SetTarget(_current_target, _get_attack());
    }
}
