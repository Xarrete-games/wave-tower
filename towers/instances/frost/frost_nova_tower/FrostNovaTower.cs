using Godot;

[GlobalClass]
public partial class FrostNovaTower : Tower
{
    private static readonly PackedScene FrostNovaProjectileScene = GD.Load<PackedScene>("uid://csif0nju31dcs");
    public float double_shot_chance = 0;

    private Marker2D projectil_spawn_point;

    public override void _Ready()
    {
        base._Ready();
        this.projectil_spawn_point = GetNode<Marker2D>("ProjectilSpawnPoint");
    }

    protected override async void _fire()
    {
        this.cristal_light?.play();

        Node projectile = FrostNovaProjectileScene.Instantiate();
        bool isDoubleHit = this._is_doble_hit();

        GodotObject attack = this._get_attack();
        float attackRange = this.stats?.attack_range ?? 0f;
        projectile.Call("set_stats", attack, attackRange, EnemyDebuff.create_frost(this.damage_source));
        CallDeferred(MethodName._add_projectil, projectile);

        if (!isDoubleHit)
        {
            return;
        }

        await ToSignal(GetTree().CreateTimer(0.5f, false), Timer.SignalName.Timeout);

        this.cristal_light?.play();
        projectile = FrostNovaProjectileScene.Instantiate();
        attack = this._get_attack();
        projectile.Call("set_stats", attack, attackRange, EnemyDebuff.create_frost(this.damage_source));
        CallDeferred(MethodName._add_projectil, projectile);
    }

    private void _add_projectil(Node projectile)
    {
        AddChild(projectile);
        if (projectile is Node2D node2D)
        {
            node2D.Position = this.projectil_spawn_point.Position;
        }
    }

    private bool _is_doble_hit()
    {
        float random = GD.Randf();
        return (this.double_shot_chance / 100f) >= random;
    }
}
