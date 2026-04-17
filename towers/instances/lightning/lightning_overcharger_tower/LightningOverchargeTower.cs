using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class LightningOverchargeTower : Tower
{
    [ExportGroup("Scenes")]
    [Export] public PackedScene projectile_scene;
    [Export] public PackedScene overcharge_particle_scene;

    public readonly List<Tower> towers_in_range = new();
    public readonly Dictionary<string, Node> particles_dict = new();

    private Marker2D projectile_spawn_pos;
    private CollisionPolygon2D buff_area_shape;
    private Area2D buff_area;
    private TowersManager _towersManager;

    public override void _Ready()
    {
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
        this.buff_area_shape = GetNode<CollisionPolygon2D>("BuffArea/BuffAreaShape");
        this.buff_area = GetNode<Area2D>("BuffArea");

        base._Ready();

        this.buff_area.Monitoring = false;
        this._apply_stats_changes();
    }

    public override void _ExitTree()
    {
        if (this._towersManager != null)
        {
            this._towersManager.tower_placed -= this._on_tower_placed;
            this._towersManager = null;
        }

        base._ExitTree();
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.projectile_scene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = this.projectile_scene.Instantiate<SingleTargetProjectile>();
        GetParent().AddChild(projectile);

        projectile.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;

        projectile.set_target(this._current_target, this._get_attack());
    }

    public override void placement_mode()
    {
        base.placement_mode();
        this.buff_area.Monitoring = false;
    }

    public override void enable()
    {
        base.enable();
        this.buff_area.Monitoring = true;

        if (this._towersManager == null)
        {
            this._towersManager = GetTowersManager();
            if (this._towersManager != null)
            {
                this._towersManager.tower_placed += this._on_tower_placed;
            }
        }
    }

    private async void _on_tower_placed(Tower tower)
    {
        if (tower == this)
        {
            return;
        }

        this.buff_area.Monitoring = false;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        this.buff_area.Monitoring = true;
    }

    public override void _apply_stats_changes()
    {
        base._apply_stats_changes();

        if (this.stats == null || this.buff_area_shape == null)
        {
            return;
        }

        float attackRange = this.stats.attack_range;
        this.buff_area_shape.SetDeferred("polygon", build_ellipse_polygon(attackRange, attackRange * ELLIPSE_Y_RATIO));
    }

    private void _on_buff_area_area_entered(Area2D area)
    {
        Tower tower = area.GetParent() as Tower;
        this._apply_buff(tower);
    }

    private void _on_buff_area_area_exited(Area2D area)
    {
        Tower tower = area.GetParent() as Tower;
        this._remove_buff(tower);
    }

    private void _apply_buff(Tower tower)
    {
        if (tower == null || tower == this || this.towers_in_range.Contains(tower))
        {
            return;
        }

        Source source = new(Source.SourceType.TOWER, Name);

        TowerBuff towerBuff = TowerBuffFactory.create_from_id("damage_mult_buff", source, 10);
        if (towerBuff == null)
        {
            return;
        }

        Node buffParticle = this.overcharge_particle_scene?.Instantiate();
        tower.add_buff(towerBuff);
        this.towers_in_range.Add(tower);

        if (buffParticle != null)
        {
            tower.AddChild(buffParticle);
            this.particles_dict[tower.Name] = buffParticle;
        }
    }

    private void _remove_buff(Tower tower)
    {
        if (tower == null || !this.towers_in_range.Contains(tower))
        {
            return;
        }

        tower.remove_buff(Name);
        this.towers_in_range.Remove(tower);

        if (!this.particles_dict.TryGetValue(tower.Name, out Node particle) || particle == null)
        {
            return;
        }

        particle.QueueFree();
        this.particles_dict.Remove(tower.Name);
    }
}
