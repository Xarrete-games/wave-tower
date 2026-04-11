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

    private void _on_enemy_killed(Variant enemy, Variant attack)
    {
        GodotObject enemyObj = enemy.AsGodotObject();
        GodotObject attackObj = attack.AsGodotObject();
        if (enemyObj == null || attackObj == null)
        {
            return;
        }

        GodotObject source = attackObj.Get("source").AsGodotObject();
        if (source == null)
        {
            return;
        }

        string typeId = source.Get("type_id").AsString();
        if (typeId != "WildFireTower")
        {
            return;
        }

        Vector2 position = enemyObj.Get("global_position").AsVector2();
        this._spawn_burn_area(position, source);
    }

    private void _spawn_burn_area(Vector2 position, GodotObject source)
    {
        if (this.burn_area_scene == null)
        {
            return;
        }

        Node burnArea = this.burn_area_scene.Instantiate();
        AddChild(burnArea);
        burnArea.Set("global_position", position);
        burnArea.Call("setup", source);
    }
}
