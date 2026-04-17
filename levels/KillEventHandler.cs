using Godot;

[GlobalClass]
public partial class KillEventHandler : Node
{
    [Export]
    public PackedScene burn_area_scene;

    private EnemyManager _enemyManager;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._enemyManager = runContext?.enemy_manager;
        if (this._enemyManager != null)
        {
            this._enemyManager.enemy_die += this._on_enemy_killed;
        }
    }

    public override void _ExitTree()
    {
        if (this._enemyManager != null)
        {
            this._enemyManager.enemy_die -= this._on_enemy_killed;
        }
    }

    private void _on_enemy_killed(object enemy, object attack)
    {
        Enemy enemyObj = enemy as Enemy;
        Attack attackObj = attack as Attack;
        if (enemyObj == null || attackObj == null)
        {
            return;
        }

        Source source = attackObj.source;
        if (source == null)
        {
            return;
        }

        string typeId = source.TypeId;
        if (typeId != "WildFireTower")
        {
            return;
        }

        Vector2 position = enemyObj.GlobalPosition;
        this._spawn_burn_area(position, source);
    }

    private void _spawn_burn_area(Vector2 position, Source source)
    {
        if (this.burn_area_scene == null)
        {
            return;
        }

        BurnArea burnArea = this.burn_area_scene.Instantiate<BurnArea>();
        AddChild(burnArea);
        burnArea.GlobalPosition = position;
        burnArea.setup(source);
    }
}
