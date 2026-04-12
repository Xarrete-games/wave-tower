using Godot;

[GlobalClass]
public partial class FrostSpearTower : Tower
{
    private static readonly Script EnemyDebuffScript = GD.Load<Script>("res://enemies/enemy_debuff/enemy_debuff.gd");

    [Export] public PackedScene frost_spear_projectile_scene;
    [Export] public int debuff_stacks = 2;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.frost_spear_projectile_scene == null)
        {
            return;
        }

        Node projectile = this.frost_spear_projectile_scene.Instantiate();
        AddChild(projectile);

        if (projectile is Node2D node2D)
        {
            node2D.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;
        }

        GodotObject attack = this._get_attack();
        int enemyFrostStacks = this._current_target.Call("get_debuff_stacks", 0).AsInt32();
        float damageMultiplier = 1.0f + enemyFrostStacks * 0.10f;
        float damage = attack.Get("damage").AsSingle();
        attack.Set("damage", damage * damageMultiplier);

        Variant debuff = EnemyDebuffScript.Call("create_frost", this.damage_source);
        projectile.Call("set_target", this._current_target, attack, debuff, this.debuff_stacks);
    }
}
