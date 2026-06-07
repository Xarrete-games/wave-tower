using Godot;

[GlobalClass]
public partial class LightningTower : TowerNode
{
    [Export] public PackedScene ElectricBallScene;

    private Marker2D _projectileSpawnPos;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || ElectricBallScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = ElectricBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = _projectileSpawnPos.GlobalPosition;

        projectile.SetTarget(_currentTarget, GetAttack());
    }
}
