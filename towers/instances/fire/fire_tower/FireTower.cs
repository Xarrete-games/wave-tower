using Godot;

[GlobalClass]
public partial class FireTower : TowerNode
{
    [Export] public PackedScene FireBallScene;

    public bool ApplyBurn = false;

    private Marker2D _projectileSpawnPos;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || FireBallScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = FireBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = _projectileSpawnPos.GlobalPosition;

        Attack attack = GetAttack();
        EnemyDebuff debuff = ApplyBurn ? EnemyDebuff.CreateBurn(DamageSource) : null;
        projectile.SetTarget(_currentTarget, attack, debuff);
    }
}
