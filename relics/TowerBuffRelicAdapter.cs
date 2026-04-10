using Godot;

public abstract partial class TowerBuffRelicAdapter : RelicRuntimeAdapter
{
    public override void on_obtain()
    {
        foreach (GodotObject tower in this.GetAllTowers())
        {
            if (this.IsValidTower(tower))
            {
                this.AddBuff(tower);
            }
        }
    }

    public override void on_tower_placed(Variant towerInstance)
    {
        GodotObject tower = towerInstance.AsGodotObject();
        if (tower == null)
        {
            return;
        }

        if (this.IsValidTower(tower))
        {
            this.AddBuff(tower);
        }
    }

    public override void on_remove()
    {
        foreach (GodotObject tower in this.GetAllTowers())
        {
            if (this.IsValidTower(tower))
            {
                this.RemoveBuff(tower);
            }
        }
    }

    protected abstract void AddBuff(GodotObject tower);
    protected abstract void RemoveBuff(GodotObject tower);
    protected abstract bool IsValidTower(GodotObject tower);

    private System.Collections.Generic.IEnumerable<GodotObject> GetAllTowers()
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject towersManager = runContext?.Get("towers_manager").AsGodotObject();
        if (towersManager == null)
        {
            yield break;
        }

        Godot.Collections.Array<Variant> towers = towersManager.Get("towers").AsGodotArray<Variant>();
        for (int index = 0; index < towers.Count; index++)
        {
            GodotObject tower = towers[index].AsGodotObject();
            if (tower != null)
            {
                yield return tower;
            }
        }
    }
}
