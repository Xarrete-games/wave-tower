using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class LightningOverchargeTower : Tower
{
    [ExportGroup("Scenes")]
    [Export] public PackedScene ProjectileScene;
    [Export] public PackedScene OverchargeParticleScene;

    private readonly List<Tower> _towersInRange = new();
    private readonly Dictionary<string, Node> _particlesByTowerName = new();

    private Marker2D _projectileSpawnPos;
    private CollisionPolygon2D _buffAreaShape;
    private Area2D _buffArea;
    private TowersManager _towersManager;

    public override void _Ready()
    {
        _projectileSpawnPos = GetNode<Marker2D>("ProjectileSpawnPos");
        _buffAreaShape = GetNode<CollisionPolygon2D>("BuffArea/BuffAreaShape");
        _buffArea = GetNode<Area2D>("BuffArea");

        base._Ready();

        _buffArea.Monitoring = false;
        ApplyStatsChanges();
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

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || ProjectileScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = ProjectileScene.Instantiate<SingleTargetProjectile>();
        GetParent().AddChild(projectile);

        projectile.GlobalPosition = _projectileSpawnPos.GlobalPosition;

        projectile.SetTarget(_currentTarget, GetAttack());
    }

    public override void PlacementMode()
    {
        base.PlacementMode();
        _buffArea.Monitoring = false;
    }

    public override void Enable()
    {
        base.Enable();
        _buffArea.Monitoring = true;

        if (_towersManager == null)
        {
            _towersManager = GetTowersManager();
            if (_towersManager != null)
            {
                _towersManager.TowerPlaced += OnTowerPlaced;
            }
        }
    }

    private void OnTowerPlaced(Tower tower)
    {
        AsyncTaskHelper.FireAndForget(OnTowerPlacedAsync(tower), "LightningOverchargeTower.OnTowerPlacedAsync");
    }

    private async Task OnTowerPlacedAsync(Tower tower)
    {
        if (tower == this)
        {
            return;
        }

        _buffArea.Monitoring = false;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _buffArea.Monitoring = true;
    }

    public override void ApplyStatsChanges()
    {
        base.ApplyStatsChanges();

        if (Stats == null || _buffAreaShape == null)
        {
            return;
        }

        float attackRange = Stats.AttackRange;
        _buffAreaShape.SetDeferred("polygon", BuildEllipsePolygon(attackRange, attackRange * ELLIPSE_Y_RATIO));
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
        if (tower == null || tower == this || _towersInRange.Contains(tower))
        {
            return;
        }

        Source source = new(Source.SourceType.TOWER, Name);

        TowerBuff towerBuff = TowerBuffFactory.CreateFromId("damage_mult_buff", source, 10);
        if (towerBuff == null)
        {
            return;
        }

        Node buffParticle = OverchargeParticleScene?.Instantiate();
        tower.AddBuff(towerBuff);
        _towersInRange.Add(tower);

        if (buffParticle != null)
        {
            tower.AddChild(buffParticle);
            _particlesByTowerName[tower.Name] = buffParticle;
        }
    }

    private void RemoveBuff(Tower tower)
    {
        if (tower == null || !_towersInRange.Contains(tower))
        {
            return;
        }

        tower.RemoveBuff(Name);
        _towersInRange.Remove(tower);

        if (!_particlesByTowerName.TryGetValue(tower.Name, out Node particle) || particle == null)
        {
            return;
        }

        particle.QueueFree();
        _particlesByTowerName.Remove(tower.Name);
    }
}
