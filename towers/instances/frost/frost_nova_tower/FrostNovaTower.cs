using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class FrostNovaTower : Tower
{
    private static readonly PackedScene FrostNovaProjectileScene = GD.Load<PackedScene>("uid://csif0nju31dcs");
    public float DoubleShotChance = 0;

    private Marker2D _projectileSpawnPoint;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPoint = GetNode<Marker2D>("ProjectilSpawnPoint");
    }

    protected override void Fire()
    {
        AsyncTaskHelper.FireAndForget(FireAsync(), "FrostNovaTower.FireAsync");
    }

    private async Task FireAsync()
    {
        _cristalLight?.Play();

        BlueProjectil projectile = FrostNovaProjectileScene.Instantiate<BlueProjectil>();
        bool isDoubleHit = IsDoubleHit();

        Attack attack = GetAttack();
        float attackRange = Stats?.AttackRange ?? 0f;
        projectile.SetStats(attack, attackRange, EnemyDebuff.CreateFrost(DamageSource));
        CallDeferred(MethodName.AddProjectile, projectile);

        if (!isDoubleHit)
        {
            return;
        }

        await ToSignal(GetTree().CreateTimer(0.5f, false), Timer.SignalName.Timeout);

        _cristalLight?.Play();
        projectile = FrostNovaProjectileScene.Instantiate<BlueProjectil>();
        attack = GetAttack();
        projectile.SetStats(attack, attackRange, EnemyDebuff.CreateFrost(DamageSource));
        CallDeferred(MethodName.AddProjectile, projectile);
    }

    private void AddProjectile(Node projectile)
    {
        AddChild(projectile);
        if (projectile is Node2D node2D)
        {
            node2D.Position = _projectileSpawnPoint.Position;
        }
    }

    private bool IsDoubleHit()
    {
        float random = GD.Randf();
        return (DoubleShotChance / 100f) >= random;
    }
}
