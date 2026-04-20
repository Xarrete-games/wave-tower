using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class LightningOverchargeTower : Tower
{
    [ExportGroup("Scenes")]
    [Export] public PackedScene ProjectileScene;
    [Export] public PackedScene OverchargeParticleScene;

    public readonly List<Tower> towers_in_range = new();
    public readonly Dictionary<string, Node> particles_dict = new();

    private Marker2D projectile_spawn_pos;
    private CollisionPolygon2D buff_area_shape;
    private Area2D buff_area;
    private TowersManager _towersManager;

    public override void _Ready()
    {
        projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
        buff_area_shape = GetNode<CollisionPolygon2D>("BuffArea/BuffAreaShape");
        buff_area = GetNode<Area2D>("BuffArea");

        base._Ready();

        buff_area.Monitoring = false;
        _apply_stats_changes();
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.TowerPlaced -= OnTowerPlaced;
            _towersManager = null;
        }

        base._ExitTree();
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || ProjectileScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = ProjectileScene.Instantiate<SingleTargetProjectile>();
        GetParent().AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        projectile.SetTarget(_current_target, _get_attack());
    }

    public override void PlacementMode()
    {
        base.PlacementMode();
        buff_area.Monitoring = false;
    }

    public override void Enable()
    {
        base.Enable();
        buff_area.Monitoring = true;

        if (_towersManager == null)
        {
            _towersManager = GetTowersManager();
            if (_towersManager != null)
            {
                _towersManager.TowerPlaced += OnTowerPlaced;
            }
        }
    }

    private async void OnTowerPlaced(Tower tower)
    {
        if (tower == this)
        {
            return;
        }

        buff_area.Monitoring = false;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        buff_area.Monitoring = true;
    }

    public override void _apply_stats_changes()
    {
        base._apply_stats_changes();

        if (stats == null || buff_area_shape == null)
        {
            return;
        }

        float attackRange = stats.attack_range;
        buff_area_shape.SetDeferred("polygon", BuildEllipsePolygon(attackRange, attackRange * ELLIPSE_Y_RATIO));
    }

    private void OnBuffAreaAreaEntered(Area2D area)
    {
        Tower tower = area.GetParent() as Tower;
        ApplyBuff(tower);
    }

    private void OnBuffAreaAreaExited(Area2D area)
    {
        Tower tower = area.GetParent() as Tower;
        RemoveBuff(tower);
    }

    private void ApplyBuff(Tower tower)
    {
        if (tower == null || tower == this || towers_in_range.Contains(tower))
        {
            return;
        }

        Source source = new(Source.SourceType.TOWER, Name);

        TowerBuff towerBuff = TowerBuffFactory.create_from_id("damage_mult_buff", source, 10);
        if (towerBuff == null)
        {
            return;
        }

        Node buffParticle = OverchargeParticleScene?.Instantiate();
        tower.AddBuff(towerBuff);
        towers_in_range.Add(tower);

        if (buffParticle != null)
        {
            tower.AddChild(buffParticle);
            particles_dict[tower.Name] = buffParticle;
        }
    }

    private void RemoveBuff(Tower tower)
    {
        if (tower == null || !towers_in_range.Contains(tower))
        {
            return;
        }

        tower.RemoveBuff(Name);
        towers_in_range.Remove(tower);

        if (!particles_dict.TryGetValue(tower.Name, out Node particle) || particle == null)
        {
            return;
        }

        particle.QueueFree();
        particles_dict.Remove(tower.Name);
    }
}
