using Godot;

[GlobalClass]
public partial class KillEventHandler : Node
{
    [Export]
    public PackedScene BurnAreaScene;

    private EnemyManager _enemyManager;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _enemyManager = runContext?.EnemyManager;
        if (_enemyManager != null)
        {
            _enemyManager.EnemyDied += OnEnemyKilled;
        }
    }

    public override void _ExitTree()
    {
        if (_enemyManager != null)
        {
            _enemyManager.EnemyDied -= OnEnemyKilled;
        }
    }

    private void OnEnemyKilled(Enemy enemy, Attack attack)
    {
        if (enemy == null || attack == null)
        {
            return;
        }

        Source source = attack.Source;
        if (source == null)
        {
            return;
        }

        string typeId = source.TypeId;
        if (typeId != "WildFireTower")
        {
            return;
        }

        Vector2 position = enemy.GlobalPosition;
        SpawnBurnArea(position, source);
    }

    private void SpawnBurnArea(Vector2 position, Source source)
    {
        if (BurnAreaScene == null)
        {
            return;
        }

        BurnArea burnArea = BurnAreaScene.Instantiate<BurnArea>();
        AddChild(burnArea);
        burnArea.GlobalPosition = position;
        burnArea.setup(source);
    }
}
